﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Senparc.AI.Kernel;
using Senparc.Areas.Admin.Domain;
using Senparc.CO2NET.Trace;
using Senparc.Ncf.Core.Config;
using Senparc.Ncf.Core.Models;
using Senparc.Ncf.Core.MultiTenant;
using Senparc.Xncf.Installer.Domain.Dto;
using Senparc.Xncf.Installer.OHS.Local.AppService;
using Senparc.Xncf.Tenant.Domain.DataBaseModel;
using System;
using System.Threading.Tasks;

namespace Senparc.Xncf.Instraller.Pages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel //不使用基类，因为无法通过已安装程序自动检测
    {
        private readonly AdminUserInfoService _accountInfoService;
        private readonly InstallAppService _installAppService;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; }
        /// <summary>
        /// 管理员用户名
        /// </summary>
        public string AdminUserName { get; set; }
        /// <summary>
        /// 管理员密码
        /// </summary>
        public string AdminPassword { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string DbConnectionString { get; set; }
        /// <summary>
        /// 需要修改的命名空间
        /// </summary>
        public string Namespace { get; set; }
        public int Step { get; set; }


        public MultipleDatabaseType MultipleDatabaseType { get; set; }

        /// <summary>
        /// 新创建的 RequestTenantInfo
        /// </summary>
        public RequestTenantInfo CreatedRequestTenantInfo { get; set; }
        public TenantInfoDto TenantInfoDto { get; private set; }
        public TenantRule TenantRule { get; set; }
        public bool MultiTenantEnable { get; set; }

        public IndexModel(IServiceProvider serviceProvider, AdminUserInfoService accountService,
            InstallAppService installAppService)
        {
            _serviceProvider = serviceProvider;
            _accountInfoService = accountService;
            this._installAppService = installAppService;

            MultiTenantEnable = SiteConfig.SenparcCoreSetting.EnableMultiTenant;
            TenantRule = SiteConfig.SenparcCoreSetting.TenantRule;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Console.WriteLine("进入安装程序，检测是否需要初始化");

                MultipleDatabaseType = DatabaseConfigurationFactory.Instance.Current.MultipleDatabaseType;
                var adminUserInfo = await _accountInfoService.GetObjectAsync(z => true);//检查是否已初始化
                if (adminUserInfo == null)
                {
                    Console.WriteLine("需要初始化");
                    throw new Exception("需要初始化");
                }

                try
                {
                    if (Senparc.Ncf.Core.Config.SiteConfig.SenparcCoreSetting.EnableMultiTenant)
                    {
                        //判断是不是从新的域名进入
                        RequestTenantInfo currentRequestTenantInfo = MultiTenantHelper.TryGetAndCheckRequestTenantInfo(_serviceProvider, null);
                    }
                }
                catch (Exception)
                {
                    throw;
                }

            }
            catch (Exception)
            {
                Console.WriteLine("开始初始化");

                //初始化页面显示的配置项的默认值
                var result = await _installAppService.GetInstallOptionsAsync();
                SystemName = result.Data.SystemName;
                AdminUserName = result.Data.AdminUserName;
                DbConnectionString = result.Data.DbConnectionString;

                return Page();
            }

            //base.Response.StatusCode = 404;

            SenparcTrace.SendCustomLog("风险提示", "Install 被访问，已返回 404 进行混淆。如果您已经确保完成项目初始化，建议移除 Senparc.Xncf.Install 模块");

            return new StatusCodeResult(404);//已经安装完毕，且存在管理员则不进行安装
        }

        public async Task<IActionResult> OnPostAsync([FromBody] InstallRequestDto installRequestDto)
        {
            //开始安装
            var result = await _installAppService.InstallAsync(installRequestDto);
            if (result.Success != true)
            {
                if (result.Data == null)
                {
                    return new StatusCodeResult(505);
                }
                return new StatusCodeResult(result.Data.StatCode);
            }

            AdminUserName = result.Data.AdminUserName;
            AdminPassword = result.Data.AdminPassword;
            Step = result.Data.Step;

            MultiTenantEnable = SiteConfig.SenparcCoreSetting.EnableMultiTenant;

            //添加初始化多租户信息
            if (SiteConfig.SenparcCoreSetting.EnableMultiTenant)
            {
                var tenantReulst = await _installAppService.GetTenantInfoAsync();
                TenantInfoDto = tenantReulst.Data;
            }
            return Page();
        }

        public IActionResult OnGetDefaultOptions()
        {
            return new JsonResult(_installAppService.GetInstallOptionsAsync());
        }
    }
}