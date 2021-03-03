using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace VideoWebApi.Util
{
    public static class FfmpegHelper
    {
        private static System.Diagnostics.ProcessStartInfo cmdFfmpeg;
        private static System.Diagnostics.ProcessStartInfo cmdFfprobe;
        static FfmpegHelper()
        {
            cmds = new Dictionary<string, System.Diagnostics.Process> { };
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine("当前环境为linux");
                string ffmpegPath = "/usr/local/ffmpeg/bin/ffmpeg";
                string ffprobePath = "/usr/local/ffmpeg/bin/ffprobe";
                cmdFfmpeg = new System.Diagnostics.ProcessStartInfo(ffmpegPath);
                cmdFfprobe = new System.Diagnostics.ProcessStartInfo(ffprobePath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + "Ffmpeg\\ffmpeg.exe";
                string ffprobePath = AppDomain.CurrentDomain.BaseDirectory + "Ffmpeg\\ffprobe.exe";
                cmdFfmpeg = new System.Diagnostics.ProcessStartInfo(ffmpegPath);
                cmdFfprobe = new System.Diagnostics.ProcessStartInfo(ffprobePath);

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                string ffmpegPath = "/usr/bin/ffmpeg";
                string ffprobePath = "/usr/bin/ffprobe";
                cmdFfmpeg = new System.Diagnostics.ProcessStartInfo(ffmpegPath);
                cmdFfprobe = new System.Diagnostics.ProcessStartInfo(ffprobePath);
            }

            cmdFfmpeg.RedirectStandardError = false; // 输出错误
            cmdFfmpeg.RedirectStandardOutput = true; //输出打印
            cmdFfmpeg.UseShellExecute = false; //使用Shell
            cmdFfmpeg.CreateNoWindow = true;  //创建黑窗


            cmdFfprobe.RedirectStandardError = false; //set false
            cmdFfprobe.RedirectStandardOutput = true;
            cmdFfprobe.UseShellExecute = false; //set true
            cmdFfprobe.CreateNoWindow = true;  //don't need the black window
        }

        /// <summary>
        /// 获取视频信息
        /// </summary>
        /// <param name="path"></param>
        //public static async Task<VideoInfoModel> GetVideoInfo(string path)
        //{
        //    try
        //    {
        //        string command = $"-i {path} -print_format json -show_format -show_streams -show_data";
        //        cmdFfprobe.Arguments = command;

        //        System.Diagnostics.Process cmd = new System.Diagnostics.Process();
        //        cmd.StartInfo = cmdFfprobe;
        //        cmd.Start();

        //        string InfoStr = await cmd.StandardOutput.ReadToEndAsync();
        //        cmd.WaitForExit(10000);

        //        VideoInfoModel resp = JsonConvert.DeserializeObject<VideoInfoModel>(InfoStr);
        //        return resp;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }

        //}
        private static Dictionary<string, System.Diagnostics.Process> cmds;
        public static async Task<string> CloseVideo(string number)
        {
            try
            {
                if (!cmds.ContainsKey(number))
                {
                    Console.WriteLine(number + "视频不存在");
                    return "error";
                }
                var cmd = cmds[number];
                cmd.Kill();
                cmd.Close();
                cmds.Remove(number);
                Console.WriteLine(number + "视频关闭成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine("关闭视频失败："+ex.Message);
            }
            return "ok";
        }
        public static async Task<string> ShowVideo(string number, string cmdStr)
        {
            if (cmds.ContainsKey(number))
            {
                Console.WriteLine(number + "视频已存在");
                return "ok";
            }
            Task.Run(async () =>
            {
                await SendCmd(cmdStr, number);
            });
            return "ok";
        }
        public static async Task<string> ClosePlaybackVideo(string number)
        {
            number = number + "_back";
            return await CloseVideo(number);
        }
        public static async Task<string> Playback(string number, string cmdStr)
        {
            number = number + "_back";
            if (cmds.ContainsKey(number))
            {
                Console.WriteLine(number + "视频已存在");
                return "ok";
            }
            Task.Run(async () =>
            {
                await SendCmd(cmdStr, number);
            });
            return "ok";
        }
        public static async Task<string> SendCmd(string cmdStr, string key)
        {
            try
            {
                Console.WriteLine(cmdStr);
                string command = cmdStr;
                cmdFfmpeg.Arguments = command;

                System.Diagnostics.Process cmd = new System.Diagnostics.Process();
                
                cmd.StartInfo = cmdFfmpeg;
                cmd.Start();
                cmds.Add(key, cmd);
                string InfoStr = await cmd.StandardOutput.ReadToEndAsync();
                if (string.IsNullOrEmpty(InfoStr))
                {
                    cmds.Remove(key);
                }
                Console.WriteLine("输出结果：" + InfoStr);
                cmd.WaitForExit(10000);
                Console.WriteLine("开始执行。。。");
                //VideoInfoModel resp = JsonConvert.DeserializeObject<VideoInfoModel>(InfoStr);
                return InfoStr;
            }
            catch (Exception ex)
            {
                if (cmds.ContainsKey(key))
                    cmds.Remove(key);
                Console.WriteLine("接口错误：" + ex.Message);
                return null;
            }
        }
        public static async Task<string> SendCmd(string cmdStr)
        {
            try
            {
                Console.WriteLine(cmdStr);
                string command = cmdStr;
                cmdFfmpeg.Arguments = command;

                System.Diagnostics.Process cmd = new System.Diagnostics.Process();
                cmd.StartInfo = cmdFfmpeg;
                cmd.Start();

                string InfoStr = await cmd.StandardOutput.ReadToEndAsync();
                cmd.WaitForExit(10000);
                Console.WriteLine("输出结果："+InfoStr);
                //VideoInfoModel resp = JsonConvert.DeserializeObject<VideoInfoModel>(InfoStr);
                return InfoStr;
            }
            catch (Exception ex)
            {
                Console.WriteLine("接口错误："+ex.Message);
                return null;
            }
        }
        public static async Task<string> GetVideoInfo(string path)
        {
            try
            {
                string command = $"-i {path} -print_format json -show_format -show_streams -show_data";
                cmdFfprobe.Arguments = command;

                System.Diagnostics.Process cmd = new System.Diagnostics.Process();
                cmd.StartInfo = cmdFfprobe;
                cmd.Start();

                string InfoStr = await cmd.StandardOutput.ReadToEndAsync();
                cmd.WaitForExit(10000);

                //VideoInfoModel resp = JsonConvert.DeserializeObject<VideoInfoModel>(InfoStr);
                return InfoStr;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 视频截图
        /// </summary>
        /// <param name="path"></param>
        /// <param name="outPath"></param>
        public static void VideoScreenshot(string path, string outPath)
        {
            string command = $"-i {path} -y -q:v 7 -f image2 -t 0.001 {outPath}";


            cmdFfmpeg.Arguments = command;

            System.Diagnostics.Process cmd = new System.Diagnostics.Process();
            cmd.StartInfo = cmdFfmpeg;
            cmd.Start();
            cmd.WaitForExit(10000);

        }
    }
}
