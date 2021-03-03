using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoWebApi.Model;
using VideoWebApi.Util;

namespace VideoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class VideoController : ControllerBase
    {
        private readonly IConfiguration configuration;
        public VideoController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> SendCmd(string cmd)
        {
            var result = await FfmpegHelper.SendCmd(cmd);
            return result;
        }
        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<string> ShowVideo(string number)
        {
            var videoConfigs = configuration.GetSection("VideoConfig").Get<List<VideoConfigModel>>();
            var videoConfig = videoConfigs.FirstOrDefault(t => t.Num == number);
            //string cmd = "ffmpeg -rtsp_transport tcp -i  " + "\"rtsp://" + videoConfig.UserName + ":" + videoConfig.Pwd + "@" + videoConfig.Url + ":" + videoConfig.Port + "/Streaming/Channels/" + videoConfig.Num + "01\"" + " -fflags flush_packets -max_delay 2 -flags -global_header -hls_time 2 -hls_list_size 3 -vcodec copy -y  /opt/nginx-1.19.0/html/hls/video1.m3u8";
            string cmd = " -rtsp_transport tcp -i  " + "\"rtsp://" + videoConfig.UserName + ":" + videoConfig.Pwd + "@" + videoConfig.Url + ":" + videoConfig.Port + "/Streaming/Channels/" + videoConfig.Num + "01\"" + " -fflags flush_packets -max_delay 2 -flags -global_header -hls_time 2 -hls_list_size 3 -vcodec copy -y  -hls_wrap 5 /opt/nginx-1.19.0/html/hls/video" + videoConfig.Num + ".m3u8";

            var result = await FfmpegHelper.ShowVideo(number, cmd);
            return result;
        }
        /// <summary>
        /// 关闭视频
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> CloseVideo(string number)
        {
            var result = await FfmpegHelper.CloseVideo(number);
            return result;
        }

        /// <summary>
        /// 回放视频
        /// </summary>
        /// <param name="number"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<string> Playback(string number, DateTime startTime, DateTime? endTime)
        {
            var videoConfigs = configuration.GetSection("VideoConfig").Get<List<VideoConfigModel>>();
            var videoConfig = videoConfigs.FirstOrDefault(t => t.Num == number);
            //string cmd = "ffmpeg -rtsp_transport tcp -i  " + "\"rtsp://" + videoConfig.UserName + ":" + videoConfig.Pwd + "@" + videoConfig.Url + ":" + videoConfig.Port + "/Streaming/Channels/" + videoConfig.Num + "01\"" + " -fflags flush_packets -max_delay 2 -flags -global_header -hls_time 2 -hls_list_size 3 -vcodec copy -y  /opt/nginx-1.19.0/html/hls/video1.m3u8";
            var startTimeStr = startTime.ToString("yyyyMMdd") + "t" + startTime.ToString("HHmmss") + "z";
            var endTimeStr = string.Empty;
            if (endTime == null)
            {
                endTime = DateTime.Now;
            }
            endTimeStr = ((DateTime)endTime).ToString("yyyyMMdd") + "t" + ((DateTime)endTime).ToString("HHmmss") + "z";
            string cmd = " -rtsp_transport tcp -i  " + "\"rtsp://" + videoConfig.UserName + ":" + videoConfig.Pwd + "@" + videoConfig.Url + ":" + videoConfig.Port + "/Streaming/tracks/" + videoConfig.Num + "01?starttime=" + startTimeStr + (!string.IsNullOrEmpty(endTimeStr) ? "&endtime=" + endTimeStr : "") + "\"" + " -fflags flush_packets -max_delay 2 -flags -global_header -hls_time 2 -hls_list_size 3 -vcodec copy -y  -hls_wrap 5 /opt/nginx-1.19.0/html/hls/video" + videoConfig.Num + "_back" + ".m3u8";

            var result = await FfmpegHelper.Playback(number, cmd);
            return result;
        }
        /// <summary>
        /// 关闭视频
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> ClosePlayback(string number)
        {
            var result = await FfmpegHelper.ClosePlaybackVideo(number);
            return result;
        }
    }
}
