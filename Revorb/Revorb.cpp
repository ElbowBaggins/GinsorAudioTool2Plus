#include "pch.h"

#include <stdio.h>
#include <ogg/ogg.h>
#include <vorbis/codec.h>

using System::IO::MemoryStream;
using System::Runtime::InteropServices::Marshal;

namespace GinsorAudioTool2Plus {
bool g_failed;

bool copy_headers(MemoryStream ^fi, ogg_sync_state *si, ogg_stream_state *is,
                  MemoryStream ^fo, ogg_sync_state *so, ogg_stream_state *os,
                  vorbis_info *vi)
{
  auto buffer = ogg_sync_buffer(si, 4096);
  const auto managedBuffer = gcnew array<unsigned char>(4096);
  auto numread = fi->Read(managedBuffer,0, 4096);
  Marshal::Copy(managedBuffer, 0, System::IntPtr(buffer), numread);
  ogg_sync_wrote(si, numread);

  ogg_page page;
  if (ogg_sync_pageout(si, &page) != 1) {
    fprintf(stderr, "Input is not an Ogg.\n");
    return false;
  }

  ogg_stream_init(is, ogg_page_serialno(&page));
  ogg_stream_init(os, ogg_page_serialno(&page));

  if (ogg_stream_pagein(is,&page) < 0) {
    fprintf(stderr, "Error in the first page.\n");
    ogg_stream_clear(is);
    ogg_stream_clear(os);
    return false;
  }

  ogg_packet packet;
  if (ogg_stream_packetout(is,&packet) != 1) {
    fprintf(stderr, "Error in the first packet.\n");
    ogg_stream_clear(is);
    ogg_stream_clear(os);
    return false;
  }

  vorbis_comment vc;
  vorbis_comment_init(&vc);
  if (vorbis_synthesis_headerin(vi, &vc, &packet) < 0) {
    fprintf(stderr, "Error in header, probably not a Vorbis file.\n");
    vorbis_comment_clear(&vc);
    ogg_stream_clear(is);
    ogg_stream_clear(os);
    return false;
  }

  ogg_stream_packetin(os, &packet);

  int i = 0;
  while(i < 2) {
    int res = ogg_sync_pageout(si, &page);

    if (res == 0) {
      buffer = ogg_sync_buffer(si, 4096);
      System::Array::Clear(managedBuffer, 0, managedBuffer->Length);
      numread = fi->Read(managedBuffer,0, 4096);
      Marshal::Copy(managedBuffer, 0, System::IntPtr(buffer), numread);
      if (numread == 0 && i < 2) {
        fprintf(stderr, "Headers are damaged, file is probably truncated.\n");
        ogg_stream_clear(is);
        ogg_stream_clear(os);
        return false;
      }
      ogg_sync_wrote(si, 4096);
      continue;
    }

    if (res == 1) {
      ogg_stream_pagein(is, &page);
      while(i < 2) {
        res = ogg_stream_packetout(is, &packet);
        if (res == 0)
          break;
        if (res < 0) {
          fprintf(stderr, "Secondary header is corrupted.\n");
          vorbis_comment_clear(&vc);
          ogg_stream_clear(is);
          ogg_stream_clear(os);
          return false;
        }
        vorbis_synthesis_headerin(vi, &vc, &packet);
        ogg_stream_packetin(os, &packet);
        i++;
      }
    }
  }

  vorbis_comment_clear(&vc);

  while(ogg_stream_flush(os,&page)) {
    const auto headerBuffer = gcnew array<unsigned char>(page.header_len);
    const auto bodyBuffer = gcnew array<unsigned char>(page.body_len);
    Marshal::Copy(System::IntPtr(page.header), headerBuffer, 0, page.header_len);
    Marshal::Copy(System::IntPtr(page.body), bodyBuffer, 0, page.body_len);

    if (!fo->CanWrite) {
      fprintf(stderr,"Cannot write headers to output.\n");
      ogg_stream_clear(is);
      ogg_stream_clear(os);
      return false;
    }

    fo->Write(headerBuffer, 0, headerBuffer->Length);
    fo->Write(bodyBuffer, 0, bodyBuffer->Length);
  }

  return true;
}

int revorb(MemoryStream^ fi, MemoryStream^ fo)
{
  ogg_sync_state sync_in, sync_out;
  ogg_sync_init(&sync_in);
  ogg_sync_init(&sync_out);

  ogg_stream_state stream_in, stream_out;
  vorbis_info vi;
  vorbis_info_init(&vi);

  ogg_packet packet;
  ogg_page page;

  if (copy_headers(fi, &sync_in, &stream_in, fo, &sync_out, &stream_out, &vi)) {
    ogg_int64_t granpos = 0, packetnum = 0;
    int lastbs = 0;

    while(1) {
      //ogg_int64_t logstream_startgran = granpos;

      int eos = 0;
      while(!eos) {
        int res = ogg_sync_pageout(&sync_in, &page);
        if (res == 0) {
          char *buffer = ogg_sync_buffer(&sync_in, 4096);
          const auto managedBuffer = gcnew array<unsigned char>(4096);
          auto numread = fi->Read(managedBuffer,0, 4096);
          Marshal::Copy(managedBuffer, 0, System::IntPtr(buffer), numread);
          if (numread > 0)
            ogg_sync_wrote(&sync_in, numread);
          else
            eos = 2;
          continue;
        }

        if (res < 0) {
          fprintf(stderr, "Warning: Corrupted or missing data in bitstream.\n");
          g_failed = true;
        } else {
          if (ogg_page_eos(&page))
            eos = 1;
          ogg_stream_pagein(&stream_in,&page);

          while(1) {
            res = ogg_stream_packetout(&stream_in, &packet);
            if (res == 0)
              break;
            if (res < 0) {
              fprintf(stderr, "Warning: Bitstream error.\n");
              g_failed = true;
              continue;
            }

            
            //if (packet.granulepos >= 0) {
            //  granpos = packet.granulepos + logstream_startgran;
            //  packet.granulepos = granpos;
            //}
            
            int bs = vorbis_packet_blocksize(&vi, &packet);
            if (lastbs)
              granpos += (lastbs+bs) / 4;
            lastbs = bs;

            packet.granulepos = granpos;
            packet.packetno = packetnum++;
            if (!packet.e_o_s) {
              ogg_stream_packetin(&stream_out, &packet);

              ogg_page opage;
              while(ogg_stream_pageout(&stream_out, &opage)) {
                const auto headerBuffer = gcnew array<unsigned char>(opage.header_len);
                const auto bodyBuffer = gcnew array<unsigned char>(opage.body_len);
                Marshal::Copy(System::IntPtr(opage.header), headerBuffer, 0, opage.header_len);
                Marshal::Copy(System::IntPtr(opage.body), bodyBuffer, 0, opage.body_len);
                if (!fo->CanWrite) {
                  fprintf(stderr, "Unable to write page to output.\n");
                  eos = 2;
                  g_failed = true;
                  break;
                }
                fo->Write(headerBuffer, 0, headerBuffer->Length);
                fo->Write(bodyBuffer, 0, bodyBuffer->Length);
              }
            }
          }
        }
      }

      if (eos == 2)
        break;

      {
        packet.e_o_s = 1;
        ogg_stream_packetin(&stream_out, &packet);
        ogg_page opage;
        while(ogg_stream_flush(&stream_out, &opage)) {
          const auto headerBuffer = gcnew array<unsigned char>(opage.header_len);
          const auto bodyBuffer = gcnew array<unsigned char>(opage.body_len);
          Marshal::Copy(System::IntPtr(opage.header), headerBuffer, 0, opage.header_len);
          Marshal::Copy(System::IntPtr(opage.body), bodyBuffer, 0, opage.body_len);
          if (!fo->CanWrite) {
            fprintf(stderr, "Unable to write page to output.\n");
            g_failed = true;
            break;
          }
          fo->Write(headerBuffer, 0, headerBuffer->Length);
          fo->Write(bodyBuffer, 0, bodyBuffer->Length);
        }
        ogg_stream_clear(&stream_in);
        break;
      }
    }

    ogg_stream_clear(&stream_out);
  } else {
    g_failed = true;
  }

  vorbis_info_clear(&vi);

  ogg_sync_clear(&sync_in);
  ogg_sync_clear(&sync_out);


  return 0;
}
  public ref class Revorb {
  public:
  static int revorb(MemoryStream^ fi, MemoryStream^ fo) {
    return GinsorAudioTool2Plus::revorb(fi, fo);
  }
};
}