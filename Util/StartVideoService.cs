using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VideoWebApi.Model;

namespace VideoWebApi.Util
{
    /// <summary>
    /// 启动视频转换
    /// </summary>
    public class StartVideoService : IHostedService
    {
        private readonly IConfiguration configuration;
        public StartVideoService(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("启动。。。。");
            try
            {
                var videoConfigs = configuration.GetSection("VideoConfig").Get<List<VideoConfigModel>>();
                foreach (var videoConfig in videoConfigs)
                {
                    string fileNum = videoConfig.Num;
                    if (videoConfig.EquipId > 0)
                    {
                        fileNum = videoConfig.EquipId.ToString();
                    }
                    string cmd = " -rtsp_transport tcp -i  " + "\"rtsp://" + videoConfig.UserName + ":" + videoConfig.Pwd + "@" + videoConfig.Url + ":" + videoConfig.Port + "/Streaming/Channels/" + videoConfig.Num + "01\"" + " -fflags flush_packets -max_delay 2 -flags -global_header -hls_time 2 -hls_list_size 3 -vcodec copy -y  -hls_wrap 5 /opt/nginx-1.19.0/html/hls/video" + fileNum + ".m3u8";

                    //string cmd = "FFREPORT=file=ffreport_error_" + number + ".log:level=16 " + "/usr/local/ffmpeg/bin/ffmpeg -rtsp_transport tcp -i  " + "\"rtsp://" + videoConfig.UserName + ":" + videoConfig.Pwd + "@" + videoConfig.Url + ":" + videoConfig.Port + "/Streaming/Channels/" + videoConfig.Num + "01\"" + " -fflags flush_packets -max_delay 2 -flags -global_header -hls_time 2 -hls_list_size 3 -vcodec copy -y  -hls_wrap 5 /opt/nginx-1.19.0/html/hls/video" + videoConfig.Num + ".m3u8";
                    var result = await FfmpegHelper.ShowVideo(videoConfig.Num, cmd);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("停止。。。。");
            return Task.CompletedTask;
        }
    }
}
