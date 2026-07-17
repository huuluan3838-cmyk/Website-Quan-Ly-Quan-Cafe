using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace DOAN_QUANCAFE.Others
{
    public class NgrokService
    {
        public static async Task<string> GetNgrokPublicUrl()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Gọi API nội bộ của Ngrok (mặc định chạy ở port 4040)
                    var response = await client.GetAsync("http://127.0.0.1:4040/api/tunnels");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var json = JObject.Parse(content);

                        // Lấy cái link public_url đầu tiên có giao thức https
                        var publicUrl = json["tunnels"]
                            .FirstOrDefault(t => t["proto"].ToString() == "https")?["public_url"]
                            .ToString();

                        return publicUrl;
                    }
                }
            }
            catch
            {
                // Nếu Ngrok chưa bật, mặc định trả về localhost để không lỗi app
                return "https://localhost:44321";
            }
            return null;
        }
    }

}
