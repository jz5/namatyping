using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NamaTyping.Auth
{
    /// <summary>
    /// ニコ生タイピング サーバー（ニコ動の認可処理）のクライアント
    /// </summary>
    public static class Client
    {
        private const string NamaTypingAuthUrl = "https://namatyping.pronama.jp"; // MEMO: 末尾の / 無し

        /// <summary>
        /// 認可ページを開く
        /// </summary>
        /// <param name="state"></param>
        public static void OpenAuthPage(string state)
        {
            Process.Start($"{NamaTypingAuthUrl}/authorize?state={state}");
        }

        /// <summary>
        /// 認可結果のテキストをデコード
        /// </summary>
        /// <param name="resultText"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static Result DecodeResult(string resultText, string state)
        {
            var json = DecryptString(resultText.Trim(), state);
            return JsonConvert.DeserializeObject<Result>(json);
        }

        /// <summary>
        /// リフレッシュトークン
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public static async Task<Result> RefreshTokenAsync(string refreshToken)
        {
            var json = await (new HttpClient()).GetStringAsync($"{NamaTypingAuthUrl}/refresh?refreshToken={refreshToken}");
            return JsonConvert.DeserializeObject<Result>(json);
        }


        // https://dobon.net/vb/dotnet/string/encryptstring.html
        /// <summary>
        /// 暗号化された文字列を復号化する
        /// </summary>
        /// <param name="sourceString">暗号化された文字列</param>
        /// <param name="password">暗号化に使用したパスワード</param>
        /// <returns>復号化された文字列</returns>
        private static string DecryptString(string sourceString, string password)
        {
            // RijndaelManagedオブジェクトを作成
            var rijndael = new System.Security.Cryptography.RijndaelManaged();

            GenerateKeyFromPassword(password, rijndael.KeySize, out var key, rijndael.BlockSize, out var iv);
            rijndael.Key = key;
            rijndael.IV = iv;

            // 文字列をバイト型配列に戻す
            var strBytes = System.Convert.FromBase64String(sourceString);

            // 対称暗号化オブジェクトの作成
            var decryptor = rijndael.CreateDecryptor();
            // バイト型配列を復号化する
            // 復号化に失敗すると例外CryptographicExceptionが発生
            var decBytes = decryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);

            decryptor.Dispose();

            // バイト型配列を文字列に戻して返す
            return System.Text.Encoding.UTF8.GetString(decBytes);
        }

        /// <summary>
        ///  パスワードから共有キーと初期化ベクタを生成する
        /// </summary>
        /// <param name="password">基になるパスワード</param>
        /// <param name="keySize">共有キーのサイズ（ビット）</param>
        /// <param name="key">作成された共有キー</param>
        /// <param name="blockSize">初期化ベクタのサイズ（ビット）</param>
        /// <param name="iv">作成された初期化ベクタ</param>
        private static void GenerateKeyFromPassword(string password, int keySize, out byte[] key, int blockSize, out byte[] iv)
        {
            // パスワードから共有キーと初期化ベクタを作成する
            // saltを決める
            var salt = System.Text.Encoding.UTF8.GetBytes("namatyping");

            // Rfc2898DeriveBytesオブジェクトを作成する
            // 反復処理回数を指定する デフォルトで1000回
            var deriveBytes = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt) { IterationCount = 1000 };

            // 共有キーと初期化ベクタを生成する
            key = deriveBytes.GetBytes(keySize / 8);
            iv = deriveBytes.GetBytes(blockSize / 8);
        }
    }
}
