using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DoAnChuyenNganh.Models.EF
{
    public class VnPayLibrary
    {
        private SortedList<string, string> requestData = new SortedList<string, string>();
        private SortedList<string, string> responseData = new SortedList<string, string>();

        // Thêm dữ liệu request
        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                requestData.Add(key, value);
        }

        // Thêm dữ liệu response
        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                responseData.Add(key, value);
        }

        public string GetResponseData(string key)
        {
            return responseData.ContainsKey(key) ? responseData[key] : "";
        }

        // ✅ Tạo URL gửi sang VNPay (chuẩn)
        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            // Sắp xếp A-Z
            var ordered = requestData.OrderBy(x => x.Key);
            var data = new StringBuilder();
            foreach (var kv in ordered)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }

            string queryString = data.ToString().TrimEnd('&');
            string signData = HmacSHA512(hashSecret, queryString);

            // Ghép URL hoàn chỉnh
            string paymentUrl = baseUrl + "?" + queryString + "&vnp_SecureHash=" + signData;

            System.Diagnostics.Debug.WriteLine("VNPay RawData: " + queryString);
            System.Diagnostics.Debug.WriteLine("VNPay Hash: " + signData);

            return paymentUrl;
        }




        // ✅ Xác minh chữ ký phản hồi (đã fix lỗi sai hash do vnp_ReturnUrl)
        public bool ValidateSignature(string inputHash, string secretKey)
        {
            if (responseData.ContainsKey("vnp_ReturnUrl"))
                responseData.Remove("vnp_ReturnUrl");

            var ordered = responseData.OrderBy(x => x.Key);
            var data = new StringBuilder();

            foreach (var kv in ordered)
            {
                if (kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType" && !string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string rawData = data.ToString().TrimEnd('&');
            string checkHash = HmacSHA512(secretKey, rawData);

            System.Diagnostics.Debug.WriteLine(">>> [VNPay Debug] rawData = " + rawData);
            System.Diagnostics.Debug.WriteLine(">>> [VNPay Debug] checkHash = " + checkHash);

            return inputHash.Equals(checkHash, StringComparison.InvariantCultureIgnoreCase);
        }


        // ✅ Hàm mã hóa HMACSHA512
        private static string HmacSHA512(string key, string inputData)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                StringBuilder hex = new StringBuilder(hashValue.Length * 2);
                foreach (byte b in hashValue)
                    hex.AppendFormat("{0:x2}", b);
                return hex.ToString();
            }
        }
    }

    // ✅ Hàm lấy IP
    public static class Utils
    {
        public static string GetIpAddress()
        {
            var context = HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = context.Request.ServerVariables["REMOTE_ADDR"];

            // 👉 Nếu là IPv6 (::1) thì đổi sang IPv4 (127.0.0.1)
            if (ipAddress == "::1")
                ipAddress = "127.0.0.1";

            return ipAddress;
        }
    }
}