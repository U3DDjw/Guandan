using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;
using HttpUtilityContains;

public class RSAVerify
{
    public static readonly string rsaPrivateKey = @"<RSAKeyValue><Modulus>7ib/shDZX/d9T7p3sXnbFIFbPV3Wzv7yD8qw/zC+mOLyVaJwNhLGnPKapejNEnvj6QyonaWHTvmP9uzpBpSRKEe8HQApGPYcPiiSKemtTTHP3NISV291BJv1JmmpCt+Z+2xktTfIL0mUJR6+BIdXgiENYO1T0a1cgs2336dRDjM=</Modulus><Exponent>AQAB</Exponent><P>+wsc+2oxZaeBR75mqV2ADGdL7sVvlYHbErHYWa/oDIce0qfyupSPEsWXHky5KEwBZtvJWcocT0xooIk9nvGkLQ==</P><Q>8tq6cdmeb2gClNcx8d5y6Lw07Nm2ggoKDK3fZxrwb2Gk/7j8T0mgh/y6UV2Eq57Z3/1FUgVUheevv0WfLj8X3w==</Q><DP>2+egO5uKKcHRPUdMJpAA2tyhZ0cLt6tIe9fN7MJqQo8aMO1tcoMv0QHEnoYWq3XoxHFCr6Sbp9sGy5lsoQ6LAQ==</DP><DQ>qzTcIAslELEosLmM8lC6fazBOwC5E6/0KcAiMNPjEQKricRly5fCXlwjFd1x/HN+cdRhnFM5pTUSxytT7wx3zQ==</DQ><InverseQ>cHu+HAL/9LrvdSraRdVbI7ukOMcZ46MWsLi9JUZ+1PXxMG/QdRgR1baSe/8nC7CwyJ83Va2MgVMpTYMqcyuYqQ==</InverseQ><D>sjvNsd6aURBQYJMVcWXZaIdHWa4ZTeHQF7GCtfotKw7uftiLUmzK6DtJMlIA6IyADbLDnvh7Q8fSzuKPo7UczK7/hiJiJ1DszaCA7d7fRsGR3WGc5Jj+h+odjCwl32BtCUfi+zbxocGcl+i3zEYMUvOLpOQTxsb/uGd5tAC7n8E=</D></RSAKeyValue>";

    private StringBuilder sendPrivateKey = new StringBuilder();

    /// <summary>
    /// RSA加密(使用公有密钥进行加密)
    /// </summary>
    /// <param name="privatekey">私有密钥</param>
    /// <param name="content">要加密的字符串</param>
    /// <returns>加密后的字符串</returns>
    public static string RSAEncrypt(string content)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        byte[] cipherbytes;
        rsa.FromXmlString(rsaPrivateKey);
        cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);
        return Convert.ToBase64String(cipherbytes);
    }

    ///// <summary>
    ///// RSA解密
    ///// </summary>
    ///// <param name="publickey">私有密钥</param>
    ///// <param name="content">私有密钥加密过的字符串</param>
    ///// <returns>解密后的字符串</returns>
    //public static string RSADecrypt(string privatekey, string content)
    //{
    //    try
    //    {
    //        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

    //        byte[] cipherbytes;
    //        rsa.FromXmlString(privatekey);
    //        cipherbytes = rsa.Decrypt(Convert.FromBase64String(content), false);
    //        return Encoding.UTF8.GetString(cipherbytes);
    //    }
    //    catch (Exception ex)
    //    {
    //        return "";
    //    }

    //}


    /// <summary>
    /// 对字符串进行SHA1加密
    /// </summary>
    /// <param name="str_source">原字符串</param>
    /// <returns>加密后的字符串</returns>
    static string getHash(string str_source)
    {
        HashAlgorithm ha = HashAlgorithm.Create("SHA1");
        byte[] bytes = Encoding.GetEncoding(0).GetBytes(str_source);
        byte[] str_hash = ha.ComputeHash(bytes);
        return Convert.ToBase64String(str_hash);
    }

    /// <summary>
    /// 对SHA1加密后的字符串进行RSA签名
    /// </summary>
    /// <param name="privatekey">私有密钥</param>
    /// <param name="str_HashbyteSingture">SHA1加密后的字符串</param>
    /// <returns>签名后的数据</returns>
     public static string SignatureFormatter(string str_source)
    {
        string str_HashbyteSingture = getHash(str_source);
        byte[] rgbHash = Convert.FromBase64String(str_HashbyteSingture);
        RSACryptoServiceProvider key = new RSACryptoServiceProvider();
        key.FromXmlString(rsaPrivateKey);
        RSAPKCS1SignatureFormatter rsa_formatter = new RSAPKCS1SignatureFormatter(key);
        rsa_formatter.SetHashAlgorithm("SHA1");
        byte[] bytes = rsa_formatter.CreateSignature(rgbHash);
        return Convert.ToBase64String(bytes);
    }

    #region 校验排序字符串，规则按照字符串首字母排序。(ascal值小的放在前面)

    public static void SortSignatureList(List<string> list)
    {
        int m = list.Count;
        //找出ascall最小的值，交换位置
        for (int i = 0; i < list.Count; i++)
        {
            m -= 1;
            for (int j = 0; j < m; j++)
            {
                if (CompareBigger(list[j], list[j + 1]))
                {
                    var temp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = temp;
                }
            }
        }
    }

    
    /// <summary>
    /// 是否含中文
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    static bool IsContainsChineseChar(string str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            return (int)str[i] > 127;
        }
        return false;
            
    }


    static void UpdateWWWForm(WWWForm www, Dictionary<string, string> dic)
    {
        foreach(var v in dic)
        {
            www.AddField(v.Key, v.Value);
        }
    }

    /// <summary>
    /// 加密Rsa+如果含中文需要转utf-8编码处理
    /// </summary>
    /// <param name="www"></param>
    /// <param name="dic"></param>
    /// <returns></returns>
    public static string SerifizationSignature(WWWForm www,Dictionary<string,string> dic)
    {
        UpdateWWWForm(www, dic);

        var list = new List<string>();
        foreach(var key in dic.Keys)
        {
            list.Add(key);
        }

        int m = list.Count;
        //找出ascall最小的值，交换位置
        for(int i=0;i<list.Count;i++)
        {
            m -= 1;
            for(int j=0;j<m;j++)
            {
                if(CompareBigger(list[j],list[j+1]))
                {
                    var temp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = temp;
                }
            }
        }

        //拼接所有字符串，返回

        StringBuilder sb = new StringBuilder();
        
        for(int i=0;i<list.Count;i++)
        {
            var value = dic[list[i]];
            if (IsContainsChineseChar(value)) //对于中文需要编码处理，服务器那边才可以解析，但是Post方式 addFiled传值还是中文
            {
                sb.Append(RSAVerify.UrlEncode(value, Encoding.UTF8));
            }
            else
            {
                sb.Append(value);
            }
        }

        //加密
        return SignatureFormatter(sb.ToString());
    }


  
    /// <summary>
    /// 是否更大（需要交换）(前一个数比后一个数小，否则，交换)
    /// </summary>
    /// <param name="str"></param>
    /// <param name="strOther"></param>
    /// <returns></returns>
    static bool CompareBigger(string str,string strOther)
    {
       if(str.Length<strOther.Length)
        {
            return CompareStrs(str, strOther);
        }
        else
        {
           return  !CompareStrs(strOther, str);
        }
    }

    /// <summary>
    /// 长度小的放前面
    /// </summary>
    /// <param name="str"></param>
    /// <param name="strOther"></param>
    static bool CompareStrs(string strSmall,string strLong)
    {
        for(int i=0;i<strSmall.Length;i++)
        {
            if(strSmall[i]<strLong[i])
            {
                return false;
            }
            else if(strSmall[i] > strLong[i])
            {
                return true;
            }
        }
        return false;
    }


    public  static string UrlEncode(string temp, Encoding encoding)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < temp.Length; i++)
        {
            string t = temp[i].ToString();
            string k = HttpUtilityTool.UrlEncode(t, encoding);
            if (t == k)
            {
                stringBuilder.Append(t);
            }
            else
            {
                stringBuilder.Append(k.ToUpper());
            }
        }
        return stringBuilder.ToString();
    }

    #endregion

    /// <summary>
    /// 对RSA签名进行验证
    /// </summary>
    /// <param name="publickey">公有密钥</param>
    /// <param name="strHashbyteDeformatter">SHA1加密后的字符串</param>
    /// <param name="strDeformatterData">RSA签名后的字符串</param>
    /// <returns></returns>
    public bool SignatureDeformatter(string publickey, string strHashbyteDeformatter, string strDeformatterData)
    {
        byte[] rgbHash = Convert.FromBase64String(strHashbyteDeformatter);
        RSACryptoServiceProvider key = new RSACryptoServiceProvider();
        key.FromXmlString(publickey);
        RSAPKCS1SignatureDeformatter rsa_Deformatter = new RSAPKCS1SignatureDeformatter(key);
        rsa_Deformatter.SetHashAlgorithm("MD5");
        byte[] rgbSignature = Convert.FromBase64String(strDeformatterData);
        if (rsa_Deformatter.VerifySignature(rgbHash, rgbSignature))
        {
            return true;
        }
        return false;
    }
}
