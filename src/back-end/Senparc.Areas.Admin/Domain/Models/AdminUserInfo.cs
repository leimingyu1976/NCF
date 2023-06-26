﻿using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Helpers;
using Senparc.Ncf.Core.Exceptions;
using Senparc.Ncf.Core.Models;
using Senparc.Ncf.Core.Utility;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Unicode;

namespace Senparc.Areas.Admin.Domain.Models
{
    [Table(Register.DATABASE_PREFIX + nameof(AdminUserInfo) + "s")]//必须添加前缀，防止全系统中发生冲突
    [Serializable]
    public partial class AdminUserInfo : EntityBase<int>
    {
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string PasswordSalt { get; private set; }
        public string RealName { get; private set; }
        public string Phone { get; private set; }
        public string Note { get; private set; }
        public DateTime ThisLoginTime { get; private set; }
        public string ThisLoginIp { get; private set; }
        public DateTime LastLoginTime { get; private set; }
        public string LastLoginIp { get; private set; }

        private AdminUserInfo() { }

        public AdminUserInfo(ref string userName, ref string password, string realName, string phone, string note)
        {
            userName ??= GenerateUserName();//记录用户名
            password ??= GeneratePassword();//记录明文密码

            UserName = userName;
            PasswordSalt = GeneratePasswordSalt();//生成密码盐
            Password = GetMD5Password(password, PasswordSalt, false);//生成密码
            RealName = realName;
            Phone = phone;
            Note = note;

            var now = SystemTime.Now.LocalDateTime;
            AddTime = now;
            ThisLoginTime = now;
            LastLoginTime = now;

            //TODO：用户名及密码合规性验证
        }

        /// <summary>
        /// 生成用户名
        /// </summary>
        /// <returns></returns>
        public string GenerateUserName()
        {
            return $"SenparcCoreAdmin{new Random().Next(100).ToString("00")}";
        }

        /// <summary>
        /// 生成随机密码
        /// </summary>
        /// <returns></returns>
        public string GeneratePassword()
        {
            return Guid.NewGuid().ToString("n").Substring(0, 16);
        }

        /// <summary>
        /// 生成密码盐
        /// </summary>
        /// <returns></returns>
        public string GeneratePasswordSalt()
        {
            return DateTime.Now.Ticks.ToString();
        }

        /// <summary>
        /// 获取加盐后的 MD5 密码
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <param name="isMD5Password"></param>
        /// <returns></returns>
        [Obsolete("MD5 作为登陆凭证已经缺少安全性，请使用 GetSHA512Password() 方法")]
        public string GetMD5Password(string password, string salt, bool isMD5Password)
        {
            string md5 = password.ToUpper().Replace("-", "");
            if (!isMD5Password)
            {
                md5 = MD5.GetMD5Code(password, "").Replace("-", ""); //原始MD5
            }
            return MD5.GetMD5Code(md5, salt).Replace("-", ""); //再加密
        }

        public string GetSHA512Password(string password, string salt, bool usePasswordToken = true)
        {
            if (salt.Length < 16)
            {
                throw new NcfExceptionBase($"{nameof(salt)} 必须大于 16 位！");
            }

            var passwordToken = (usePasswordToken
                                        ? Senparc.Ncf.Core.Config.SiteConfig.SenparcCoreSetting.PasswordSaltToken
                                        : null) ?? "";

            salt += passwordToken;

            var ascii = Encoding.ASCII.GetBytes(MD5.GetMD5Code(salt, "").Replace("-", ""));

            string sha512 = password;

            var splitPoint = usePasswordToken ? Encoding.ASCII.GetBytes(passwordToken).LastOrDefault() : 50;

            for (int i = 0; i < 20; i++)
            {
                if (ascii[i] % 2 == 0)
                {
                    sha512 = EncryptHelper.GetHmacSha256(sha512, salt);
                }
                else
                {
                    if (ascii[i] > splitPoint)
                    {
                        sha512 = EncryptHelper.GetSha1(sha512);
                    }
                    else
                    {
                        sha512 = EncryptHelper.GetSha1(sha512, toUpper: true);
                    }
                }
            }

            return "g01" + sha512;
        }

        public void UpdateObject(CreateOrUpdate_AdminUserInfoDto objDto)
        {
            UserName = objDto.UserName;
            if (!objDto.Password.IsNullOrEmpty())
            {
                Password = GetMD5Password(objDto.Password, this.PasswordSalt, false);
            }

            RealName = objDto.RealName;
            Phone = objDto.Phone;
            Note = objDto.Note;

            base.SetUpdateTime();
        }
    }
}
