# HSBG.Xncf.BidManager — 标书管理系统设计方案

---

## 一、项目概述

基于 **NCF (NeuCharFramework)** 框架开发标书管理 XNCF 模块，严格遵循官方模块编码规范，覆盖标书制作全流程数字化管理。

| 项目 | 说明 |
|------|------|
| 模块名称 | HSBG.Xncf.BidManager |
| 模块 UID | D3DBB37E-6333-4F4F-BAFB-E24586FE6364 |
| 命名空间 | `HSBG.Xncf.BidManager` |
| 数据库前缀 | `HSBG_BidManager_` |
| 目标框架 | .NET 8 / ASP.NET Core |
| 前端技术 | Razor Pages + Vue.js 2.x + Element UI |
| 架构模式 | DDD 四层架构（Domain / OHS / ACL / Areas） |

---

## 二、功能模块总览

| 序号 | 模块 | Area 路径 | 说明 |
|------|------|-----------|------|
| 1 | 首页仪表盘 | `/Admin/BidManager/Index` | 关键指标总览、快捷入口 |
| 2 | 客户管理 | `/Admin/BidManager/Customer/List` | 客户信息 CRUD |
| 3 | 项目管理 | `/Admin/BidManager/Project/List` | 投标项目全生命周期 |
| 4 | 标书管理 | `/Admin/BidManager/Bid/List` | 标书制作全过程 |
| 5 | 制作进度 | `/Admin/BidManager/Progress/Index` | 进度看板与甘特图 |
| 6 | 费用管理 | `/Admin/BidManager/Expense/List` | 费用记录与统计 |
| 7 | 人员管理 | `/Admin/BidManager/Personnel/Index` | 人员与任务分配 |
| 8 | 提醒中心 | `/Admin/BidManager/Reminder/Index` | 自动提醒与通知 |
| 9 | 统计中心 | `/Admin/BidManager/Report/Index` | 数据统计与报表 |
| 10 | 系统设置 | `/Admin/BidManager/Settings/Index` | 系统参数配置 |

---

## 三、DDD 分层架构（严格遵循 NCF 官方模块风格）

### 3.1 目录结构

```
HSBG.BidManager/
│
├── Register.cs                      # partial - IXncfRegister（主注册）
├── Register.Area.cs                 # partial - IAreaRegister + IXncfRazorRuntimeCompilation
├── Register.Database.cs             # partial - IXncfDatabase
├── Register.Function.cs             # partial - IXncfFunction（按需，如统计报表）
├── HSBG.Xncf.BidManager.csproj
├── icon.jpg
├── readme.md
│
├── ACL/                             # 防腐层（Anti-Corruption Layer）
│   └── Repository/                  # 仓储（按需，ServiceBase 已提供基础 CRUD）
│       └── readme.md
│
├── Domain/                          # 领域层
│   ├── Migrations/                  # EF Core 迁移（多数据库）
│   │   ├── MySql/
│   │   ├── Oracle/
│   │   ├── PostgreSQL/
│   │   ├── Sqlite/
│   │   └── SqlServer/
│   ├── Models/
│   │   └── DatabaseModel/
│   │       ├── Customer.cs          # 客户实体
│   │       ├── Project.cs           # 项目实体
│   │       ├── Bid.cs               # 标书实体
│   │       ├── BidProgress.cs       # 制作进度实体
│   │       ├── BidExpense.cs        # 费用实体
│   │       ├── Personnel.cs         # 人员实体
│   │       ├── BidPersonnel.cs      # 标书-人员关联
│   │       ├── Reminder.cs          # 提醒实体
│   │       ├── SystemConfig.cs      # 系统配置实体
│   │       ├── BidManagerSenparcEntities.cs    # DbContext
│   │       ├── Dto/                  # 数据传输对象
│   │       │   ├── CustomerDto.cs
│   │       │   ├── ProjectDto.cs
│   │       │   ├── BidDto.cs
│   │       │   ├── BidProgressDto.cs
│   │       │   ├── BidExpenseDto.cs
│   │       │   ├── PersonnelDto.cs
│   │       │   ├── ReminderDto.cs
│   │       │   └── DashboardDto.cs  # 仪表盘聚合 DTO
│   │       ├── Mapping/             # EF 实体映射配置
│   │       │   ├── BidManager_CustomerConfigurationMapping.cs
│   │       │   ├── BidManager_ProjectConfigurationMapping.cs
│   │       │   ├── BidManager_BidConfigurationMapping.cs
│   │       │   ├── BidManager_BidProgressConfigurationMapping.cs
│   │       │   ├── BidManager_BidExpenseConfigurationMapping.cs
│   │       │   ├── BidManager_PersonnelConfigurationMapping.cs
│   │       │   ├── BidManager_BidPersonnelConfigurationMapping.cs
│   │       │   ├── BidManager_ReminderConfigurationMapping.cs
│   │       │   └── BidManager_SystemConfigConfigurationMapping.cs
│   │       └── MultipleDatabase/    # 多数据库 DbContext
│   │           ├── BidManagerSenparcEntities_Dm.cs
│   │           ├── BidManagerSenparcEntities_MySql.cs
│   │           ├── BidManagerSenparcEntities_Oracle.cs
│   │           ├── BidManagerSenparcEntities_PostgreSQL.cs
│   │           ├── BidManagerSenparcEntities_SQLite.cs
│   │           └── BidManagerSenparcEntities_SqlServer.cs
│   │
│   └── Services/                    # 领域服务（业务逻辑）
│       ├── CustomerService.cs
│       ├── ProjectService.cs
│       ├── BidService.cs
│       ├── BidProgressService.cs
│       ├── BidExpenseService.cs
│       ├── PersonnelService.cs
│       ├── ReminderService.cs
│       ├── SystemConfigService.cs
│       └── SysLogService.cs
│
├── OHS/ (Open Host Service)         # 应用服务层（API 暴露）
│   └── Local/
│       ├── AppService/              # 应用服务
│       │   ├── ApiAppService.cs     # 公共 API
│       │   ├── CustomerAppService.cs
│       │   ├── ProjectAppService.cs
│       │   ├── BidAppService.cs
│       │   ├── BidProgressAppService.cs
│       │   ├── ExpenseAppService.cs
│       │   ├── PersonnelAppService.cs
│       │   ├── ReminderAppService.cs
│       │   ├── DashboardAppService.cs
│       │   └── SettingsAppService.cs
│       └── PL/                      # 请求/响应对象
│           ├── BidRequest.cs
│           ├── BidResponse.cs
│           ├── CustomerRequest.cs
│           ├── CustomerResponse.cs
│           ├── ProjectRequest.cs
│           ├── ProjectResponse.cs
│           ├── DashboardResponse.cs
│           └── ...
│
├── Areas/Admin/Pages/BidManager/    # 前端 UI
│   ├── Index.cshtml                 # 首页仪表盘
│   ├── Index.cshtml.cs
│   ├── Customer/
│   │   ├── List.cshtml
│   │   ├── List.cshtml.cs
│   │   ├── Detail.cshtml
│   │   └── Detail.cshtml.cs
│   ├── Project/
│   │   ├── List.cshtml
│   │   ├── List.cshtml.cs
│   │   ├── Detail.cshtml
│   │   └── Detail.cshtml.cs
│   ├── Bid/
│   │   ├── List.cshtml
│   │   ├── List.cshtml.cs
│   │   ├── Detail.cshtml
│   │   └── Detail.cshtml.cs
│   ├── Progress/
│   │   ├── Index.cshtml
│   │   └── Index.cshtml.cs
│   ├── Expense/
│   │   ├── List.cshtml
│   │   ├── List.cshtml.cs
│   │   ├── Detail.cshtml
│   │   └── Detail.cshtml.cs
│   ├── Personnel/
│   │   ├── Index.cshtml
│   │   └── Index.cshtml.cs
│   ├── Reminder/
│   │   ├── Index.cshtml
│   │   └── Index.cshtml.cs
│   ├── Report/
│   │   ├── Index.cshtml
│   │   └── Index.cshtml.cs
│   ├── Settings/
│   │   ├── Index.cshtml
│   │   └── Index.cshtml.cs
│   └── Shared/
│       ├── _SideMenu.cshtml
│       ├── _ViewImports.cshtml
│       └── _ViewStart.cshtml
│
└── wwwroot/                         # 静态资源
    ├── css/
    ├── images/
    ├── js/
    └── lib/
```

### 3.2 分层职责（与官方模块保持一致）

```
┌──────────────────────────────────────────────────────────┐
│  Areas/Admin/Pages  (UI 层)                              │
│  继承 AdminXncfModulePageModelBase                        │
│  Razor Pages + Vue.js + Element UI                       │
├──────────────────────────────────────────────────────────┤
│  OHS/Local/AppService  (应用服务层)                      │
│  继承 AppServiceBase，[ApiBind] 暴露 Dynamic WebApi       │
│  方法返回 AppResponseBase<T> / StringAppResponse          │
├──────────────────────────────────────────────────────────┤
│  Domain/Services  (领域服务层)                            │
│  继承 ServiceBase<TEntity> / BaseClientService<TEntity>  │
│  封装核心业务逻辑，组合 SaveObject / DeleteObject          │
├──────────────────────────────────────────────────────────┤
│  Domain/Models  (领域模型层)                              │
│  实体继承 EntityBase<int>                                │
│  DTO 为普通 POCO 类                                      │
│  Mapping 继承 ConfigurationMappingWithIdBase<T, int>      │
│  标记 [XncfAutoConfigurationMapping]                      │
└──────────────────────────────────────────────────────────┘
```

---

## 四、核心模块 Register 设计（完全遵循官方 partial class 模式）

### 4.1 Register.cs — IXncfRegister

```csharp
// 文件：HSBG.BidManager/Register.cs
using Senparc.Ncf.Core.Enums;
using Senparc.Ncf.XncfBase;
using Senparc.Ncf.XncfBase.Database;
using Senparc.Ncf.Core.Models;
using Senparc.Ncf.Database;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using HSBG.Xncf.BidManager.Models;
using HSBG.Xncf.BidManager.OHS.Local.AppService;
using HSBG.Xncf.BidManager.Domain.Services;

namespace HSBG.Xncf.BidManager
{
    [XncfRegister]
    public partial class Register : XncfRegisterBase, IXncfRegister
    {
        #region IXncfRegister 接口

        public override string Name => "HSBG.Xncf.BidManager";

        public override string Uid => "D3DBB37E-6333-4F4F-BAFB-E24586FE6364";

        public override string Version => "0.1";

        public override string MenuName => "标书管理";

        public override string Icon => "fa fa-file-text-o";

        public override string Description => "标书制作全流程管理系统";

        public override async Task InstallOrUpdateAsync(IServiceProvider serviceProvider, InstallOrUpdate installOrUpdate)
        {
            // 安装或升级版本时更新数据库
            await XncfDatabaseDbContext.MigrateOnInstallAsync(serviceProvider, this);

            switch (installOrUpdate)
            {
                case InstallOrUpdate.Install:
                    // 新安装
                    #region 初始化数据库数据

                    // 预设制作进度阶段模板
                    var progressConfig = new SystemConfig
                    {
                        ConfigKey = "ProgressTemplate",
                        ConfigValue = "大纲编写,初稿撰写,内部审核,修改完善,定稿,排版装订,最终审核,递交",
                        Description = "标书制作进度阶段模板",
                        UpdateTime = SystemTime.Now
                    };
                    var configService = serviceProvider.GetService<SystemConfigService>();
                    configService.SaveObject(progressConfig);

                    #endregion
                    break;
                case InstallOrUpdate.Update:
                    // 更新
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override async Task UninstallAsync(IServiceProvider serviceProvider, Func<Task> unsinstallFunc)
        {
            #region 删除数据库

            var mySenparcEntitiesType = this.TryGetXncfDatabaseDbContextType;
            BidManagerSenparcEntities mySenparcEntities = serviceProvider.GetService(mySenparcEntitiesType) as BidManagerSenparcEntities;

            // 按照删除顺序对实体进行排序
            var dropTableKeys = EntitySetKeys.GetEntitySetInfo(this.TryGetXncfDatabaseDbContextType).Keys.ToArray();
            await base.DropTablesAsync(serviceProvider, mySenparcEntities, dropTableKeys);

            #endregion
            await unsinstallFunc().ConfigureAwait(false);
        }

        #endregion

        #region 服务注册

        public override IServiceCollection AddXncfModule(IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            // 领域服务
            services.AddScoped<CustomerService>();
            services.AddScoped<ProjectService>();
            services.AddScoped<BidService>();
            services.AddScoped<BidProgressService>();
            services.AddScoped<BidExpenseService>();
            services.AddScoped<PersonnelService>();
            services.AddScoped<ReminderService>();
            services.AddScoped<SystemConfigService>();
            services.AddScoped<SysLogService>();

            // 应用服务
            services.AddScoped<CustomerAppService>();
            services.AddScoped<ProjectAppService>();
            services.AddScoped<BidAppService>();
            services.AddScoped<BidProgressAppService>();
            services.AddScoped<ExpenseAppService>();
            services.AddScoped<PersonnelAppService>();
            services.AddScoped<ReminderAppService>();
            services.AddScoped<DashboardAppService>();
            services.AddScoped<SettingsAppService>();

            // AutoMapper
            services.AddAutoMapper(z =>
            {
                z.CreateMap<Customer, CustomerDto>().ReverseMap();
                z.CreateMap<Project, ProjectDto>().ReverseMap();
                z.CreateMap<Bid, BidDto>().ReverseMap();
                z.CreateMap<BidProgress, BidProgressDto>().ReverseMap();
                z.CreateMap<BidExpense, BidExpenseDto>().ReverseMap();
                z.CreateMap<Personnel, PersonnelDto>().ReverseMap();
                z.CreateMap<Reminder, ReminderDto>().ReverseMap();
            });

            return base.AddXncfModule(services, configuration, env);
        }

        #endregion
    }
}
```

### 4.2 Register.Area.cs — IAreaRegister（菜单与路由）

```csharp
// 文件：HSBG.BidManager/Register.Area.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Senparc.CO2NET.Trace;
using Senparc.Ncf.Core.Areas;
using Senparc.Ncf.Core.Config;
using Senparc.Ncf.XncfBase;
using System;
using System.Collections.Generic;
using System.IO;

namespace HSBG.Xncf.BidManager
{
    public partial class Register : IAreaRegister,
                                    IXncfRazorRuntimeCompilation
    {
        #region IAreaRegister 接口

        public string HomeUrl => "/Admin/BidManager/Index";

        public List<AreaPageMenuItem> AreaPageMenuItems => new List<AreaPageMenuItem>()
        {
            // 首页
            new AreaPageMenuItem(GetAreaHomeUrl(), "首页仪表盘", "fa fa-dashboard"),

            // 业务管理
            new AreaPageMenuItem(GetAreaUrl("/Admin/BidManager/Customer/List"), "客户管理", "fa fa-address-book"),
            new AreaPageMenuItem(GetAreaUrl("/Admin/BidManager/Project/List"), "项目管理", "fa fa-folder-open"),
            new AreaPageMenuItem(GetAreaUrl("/Admin/BidManager/Bid/List"), "标书管理", "fa fa-file-text-o"),
            new AreaPageMenuItem(GetAreaUrl("/Admin/BidManager/Progress/Index"), "制作进度", "fa fa-tasks"),

            // 辅助管理
            new AreaPageMenuItem(GetAreaUrl("/Admin/BidManager/Expense/List"), "费用管理", "fa fa-cny"),
            new AreaPageMenuItem(GetAreaUrl("/Admin/BidManager/Personnel/Index"), "人员管理", "fa fa-users"),

            // 系统
            new AreaPageMenuItem(GetAreaUrl("/Admin/BidManager/Reminder/Index"), "提醒中心", "fa fa-bell-o"),
            new AreaPageMenuItem(GetAreaUrl("/Admin/BidManager/Report/Index"), "统计中心", "fa fa-bar-chart"),
            new AreaPageMenuItem(GetAreaUrl("/Admin/BidManager/Settings/Index"), "系统设置", "fa fa-cog"),
        };

        public IMvcBuilder AuthorizeConfig(IMvcBuilder builder, IHostEnvironment env)
        {
            builder.AddRazorPagesOptions(options =>
            {
                // 此处可配置页面权限
            });

            SenparcTrace.SendCustomLog("BidManager 启动", "完成 Area:HSBG.Xncf.BidManager 注册");

            return builder;
        }

        #endregion

        #region IXncfRazorRuntimeCompilation 接口

        public string LibraryPath =>
            Path.GetFullPath(Path.Combine(SiteConfig.WebRootPath, "..", "..", "HSBG.Xncf.BidManager"));

        #endregion
    }
}
```

### 4.3 Register.Database.cs — IXncfDatabase

```csharp
// 文件：HSBG.BidManager/Register.Database.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Senparc.Ncf.Database;
using Senparc.Ncf.Core.Models;

namespace HSBG.Xncf.BidManager
{
    public partial class Register : IXncfDatabase
    {
        #region IXncfDatabase 接口

        /// <summary>
        /// 数据库前缀
        /// </summary>
        public const string DATABASE_PREFIX = "HSBG_BidManager_";

        /// <summary>
        /// 数据库前缀
        /// </summary>
        public string DatabaseUniquePrefix => DATABASE_PREFIX;

        /// <summary>
        /// 动态获取数据库上下文
        /// </summary>
        public Type TryGetXncfDatabaseDbContextType =>
            MultipleDatabasePool.Instance.GetXncfDbContextType(this);

        public void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 实现 [XncfAutoConfigurationMapping] 特性之后，可以自动执行，无需手动添加
        }

        public void AddXncfDatabaseModule(IServiceCollection services)
        {
            //DOT REMOVE OR MODIFY THIS LINE 请勿移除或修改本行 - Entities Point
        }

        #endregion
    }
}
```

---

## 五、数据模型详细设计

### 5.1 ER 关系图

```
Customer (1) ────→ (*) Project
Project  (1) ────→ (*) Bid
Bid      (1) ────→ (*) BidProgress
Bid      (1) ────→ (*) BidExpense
Bid     (*) ←───→ (*) Personnel    (via BidPersonnel)
Reminder       ─── 独立提醒表（RelatedId + RelatedType 多态关联）
SystemConfig   ─── 独立配置表
```

### 5.2 实体设计（严格遵循官方 EntityBase 模式）

#### 客户实体（Customer）

```csharp
// 文件：Domain/Models/DatabaseModel/Customer.cs
using Senparc.Ncf.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSBG.Xncf.BidManager
{
    /// <summary>
    /// 客户实体
    /// </summary>
    [Table(Register.DATABASE_PREFIX + nameof(Customer))]
    [Serializable]
    public class Customer : EntityBase<int>
    {
        /// <summary>
        /// 客户名称
        /// </summary>
        [MaxLength(200)]
        public string Name { get; private set; }

        /// <summary>
        /// 客户简称
        /// </summary>
        [MaxLength(100)]
        public string Abbreviation { get; private set; }

        /// <summary>
        /// 客户类型
        /// </summary>
        public CustomerType Type { get; private set; }

        /// <summary>
        /// 联系人
        /// </summary>
        [MaxLength(50)]
        public string ContactPerson { get; private set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [MaxLength(30)]
        public string Phone { get; private set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(100)]
        public string Email { get; private set; }

        /// <summary>
        /// 地址
        /// </summary>
        [MaxLength(500)]
        public string Address { get; private set; }

        /// <summary>
        /// 标签（JSON 存储）
        /// </summary>
        public string Tags { get; private set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(1000)]
        public string Remark { get; private set; }

        /// <summary>
        /// 是否活跃
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; private set; }

        // 导航属性
        public ICollection<Project> Projects { get; set; }

        private Customer() { }

        public Customer(string name, string contactPerson, string phone, CustomerType type = CustomerType.民营企业)
        {
            Name = name;
            ContactPerson = contactPerson;
            Phone = phone;
            Type = type;
            IsActive = true;
            AddTime = SystemTime.Now;
            LastUpdateTime = SystemTime.Now;
        }

        public void Update(string name, string abbreviation, CustomerType type,
            string contactPerson, string phone, string email, string address,
            string tags, string remark)
        {
            Name = name ?? Name;
            Abbreviation = abbreviation ?? Abbreviation;
            Type = type;
            ContactPerson = contactPerson ?? ContactPerson;
            Phone = phone ?? Phone;
            Email = email ?? Email;
            Address = address ?? Address;
            Tags = tags ?? Tags;
            Remark = remark ?? Remark;
            LastUpdateTime = SystemTime.Now;
        }

        public void SetActive(bool active)
        {
            IsActive = active;
            LastUpdateTime = SystemTime.Now;
        }
    }

    public enum CustomerType
    {
        政府机关 = 0,
        国有企业 = 1,
        民营企业 = 2,
        外资企业 = 3,
        事业单位 = 4
    }
}
```

#### 项目实体（Project）

```csharp
// 文件：Domain/Models/DatabaseModel/Project.cs
using Senparc.Ncf.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSBG.Xncf.BidManager
{
    /// <summary>
    /// 投标项目实体
    /// </summary>
    [Table(Register.DATABASE_PREFIX + nameof(Project))]
    [Serializable]
    public class Project : EntityBase<int>
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        [MaxLength(200)]
        public string ProjectName { get; private set; }

        /// <summary>
        /// 项目编号（系统自动生成）
        /// </summary>
        [MaxLength(50)]
        public string ProjectCode { get; private set; }

        /// <summary>
        /// 关联客户ID
        /// </summary>
        public int CustomerId { get; private set; }

        /// <summary>
        /// 招标类型
        /// </summary>
        public BidType BidType { get; private set; }

        /// <summary>
        /// 项目状态
        /// </summary>
        public ProjectStatus Status { get; private set; }

        /// <summary>
        /// 投标截止日期
        /// </summary>
        public DateTime? BidDeadline { get; private set; }

        /// <summary>
        /// 开标时间
        /// </summary>
        public DateTime? BidOpenTime { get; private set; }

        /// <summary>
        /// 投标金额
        /// </summary>
        public decimal? BidAmount { get; private set; }

        /// <summary>
        /// 中标金额
        /// </summary>
        public decimal? WinningAmount { get; private set; }

        /// <summary>
        /// 负责人
        /// </summary>
        [MaxLength(50)]
        public string ResponsiblePerson { get; private set; }

        /// <summary>
        /// 优先级（高/中/低）
        /// </summary>
        [MaxLength(10)]
        public string Priority { get; private set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(2000)]
        public string Remark { get; private set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; private set; }

        // 导航属性
        public Customer Customer { get; set; }
        public ICollection<Bid> Bids { get; set; }

        private Project() { }

        public Project(string projectName, string projectCode, int customerId,
            BidType bidType, string responsiblePerson)
        {
            ProjectName = projectName;
            ProjectCode = projectCode;
            CustomerId = customerId;
            BidType = bidType;
            ResponsiblePerson = responsiblePerson;
            Status = ProjectStatus.跟踪中;
            Priority = "中";
            AddTime = SystemTime.Now;
            UpdateTime = SystemTime.Now;
        }

        /// <summary>
        /// 更新项目基本信息
        /// </summary>
        public void Update(string projectName, BidType bidType, DateTime? bidDeadline,
            DateTime? bidOpenTime, decimal? bidAmount, string responsiblePerson,
            string priority, string remark)
        {
            ProjectName = projectName ?? ProjectName;
            BidType = bidType;
            BidDeadline = bidDeadline ?? BidDeadline;
            BidOpenTime = bidOpenTime ?? BidOpenTime;
            BidAmount = bidAmount ?? BidAmount;
            ResponsiblePerson = responsiblePerson ?? ResponsiblePerson;
            Priority = priority ?? Priority;
            Remark = remark ?? Remark;
            UpdateTime = SystemTime.Now;
        }

        /// <summary>
        /// 状态流转
        /// </summary>
        public void ChangeStatus(ProjectStatus newStatus)
        {
            Status = newStatus;
            UpdateTime = SystemTime.Now;
        }
    }

    /// <summary>
    /// 招标类型
    /// </summary>
    public enum BidType
    {
        公开招标 = 0,
        邀请招标 = 1,
        竞争性谈判 = 2,
        单一来源 = 3,
        询价 = 4
    }

    /// <summary>
    /// 项目状态
    /// </summary>
    public enum ProjectStatus
    {
        跟踪中 = 0,
        已立项 = 1,
        制作中 = 2,
        已投标 = 3,
        已中标 = 4,
        未中标 = 5,
        已废标 = 6
    }
}
```

#### 标书实体（Bid）

```csharp
// 文件：Domain/Models/DatabaseModel/Bid.cs
using Senparc.Ncf.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSBG.Xncf.BidManager
{
    /// <summary>
    /// 标书实体
    /// </summary>
    [Table(Register.DATABASE_PREFIX + nameof(Bid))]
    [Serializable]
    public class Bid : EntityBase<int>
    {
        /// <summary>
        /// 关联项目ID
        /// </summary>
        public int ProjectId { get; private set; }

        /// <summary>
        /// 标书编号
        /// </summary>
        [MaxLength(50)]
        public string BidCode { get; private set; }

        /// <summary>
        /// 标书名称
        /// </summary>
        [MaxLength(200)]
        public string BidName { get; private set; }

        /// <summary>
        /// 标书状态
        /// </summary>
        public BidStatus Status { get; private set; }

        /// <summary>
        /// 版本号
        /// </summary>
        [MaxLength(20)]
        public string Version { get; private set; }

        /// <summary>
        /// 计划开始日期
        /// </summary>
        public DateTime PlanStartDate { get; private set; }

        /// <summary>
        /// 计划完成日期
        /// </summary>
        public DateTime PlanEndDate { get; private set; }

        /// <summary>
        /// 实际开始日期
        /// </summary>
        public DateTime? ActualStartDate { get; private set; }

        /// <summary>
        /// 实际完成日期
        /// </summary>
        public DateTime? ActualEndDate { get; private set; }

        /// <summary>
        /// 标书内容描述
        /// </summary>
        [MaxLength(2000)]
        public string ContentDesc { get; private set; }

        /// <summary>
        /// 标书文件路径
        /// </summary>
        [MaxLength(500)]
        public string FilePath { get; private set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        [MaxLength(20)]
        public string CheckStatus { get; private set; }

        /// <summary>
        /// 总费用（汇总计算）
        /// </summary>
        public decimal? TotalExpense { get; private set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; private set; }

        // 导航属性
        public Project Project { get; set; }
        public ICollection<BidProgress> Progresses { get; set; }
        public ICollection<BidExpense> Expenses { get; set; }
        public ICollection<BidPersonnel> BidPersonnel { get; set; }

        private Bid() { }

        public Bid(int projectId, string bidCode, string bidName,
            DateTime planStartDate, DateTime planEndDate)
        {
            ProjectId = projectId;
            BidCode = bidCode;
            BidName = bidName;
            PlanStartDate = planStartDate;
            PlanEndDate = planEndDate;
            Status = BidStatus.编制中;
            Version = "V1.0";
            CheckStatus = "待审核";
            AddTime = SystemTime.Now;
            UpdateTime = SystemTime.Now;
        }

        public void Update(string bidName, DateTime planStartDate, DateTime planEndDate,
            string contentDesc, string filePath)
        {
            BidName = bidName ?? BidName;
            PlanStartDate = planStartDate;
            PlanEndDate = planEndDate;
            ContentDesc = contentDesc ?? ContentDesc;
            FilePath = filePath ?? FilePath;
            UpdateTime = SystemTime.Now;
        }

        public void ChangeStatus(BidStatus newStatus)
        {
            Status = newStatus;
            if (newStatus == BidStatus.已递交)
            {
                ActualEndDate = SystemTime.Now;
            }
            UpdateTime = SystemTime.Now;
        }

        public void UpdateCheckStatus(string status)
        {
            CheckStatus = status;
            UpdateTime = SystemTime.Now;
        }
    }

    /// <summary>
    /// 标书状态
    /// </summary>
    public enum BidStatus
    {
        编制中 = 0,
        审核中 = 1,
        审核通过 = 2,
        审核退回 = 3,
        已装订 = 4,
        已递交 = 5
    }
}
```

#### 制作进度实体（BidProgress）

```csharp
// 文件：Domain/Models/DatabaseModel/BidProgress.cs
using Senparc.Ncf.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSBG.Xncf.BidManager
{
    /// <summary>
    /// 标书制作进度实体
    /// </summary>
    [Table(Register.DATABASE_PREFIX + nameof(BidProgress))]
    [Serializable]
    public class BidProgress : EntityBase<int>
    {
        /// <summary>
        /// 关联标书ID
        /// </summary>
        public int BidId { get; private set; }

        /// <summary>
        /// 阶段名称
        /// </summary>
        [MaxLength(100)]
        public string Stage { get; private set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderIndex { get; private set; }

        /// <summary>
        /// 进度状态
        /// </summary>
        public ProgressStatus Status { get; private set; }

        /// <summary>
        /// 负责人
        /// </summary>
        [MaxLength(50)]
        public string Assignee { get; private set; }

        /// <summary>
        /// 计划开始日期
        /// </summary>
        public DateTime? PlanStartDate { get; private set; }

        /// <summary>
        /// 计划完成日期
        /// </summary>
        public DateTime? PlanEndDate { get; private set; }

        /// <summary>
        /// 实际开始日期
        /// </summary>
        public DateTime? ActualStartDate { get; private set; }

        /// <summary>
        /// 实际完成日期
        /// </summary>
        public DateTime? ActualEndDate { get; private set; }

        /// <summary>
        /// 完成百分比（0-100）
        /// </summary>
        public int ProgressPercent { get; private set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Remark { get; private set; }

        // 导航属性
        public Bid Bid { get; set; }

        private BidProgress() { }

        public BidProgress(int bidId, string stage, int orderIndex, string assignee)
        {
            BidId = bidId;
            Stage = stage;
            OrderIndex = orderIndex;
            Assignee = assignee;
            Status = ProgressStatus.未开始;
            ProgressPercent = 0;
            AddTime = SystemTime.Now;
        }

        /// <summary>
        /// 更新进度状态
        /// </summary>
        public void UpdateStatus(ProgressStatus status, int progressPercent, string remark)
        {
            Status = status;
            ProgressPercent = progressPercent;
            Remark = remark ?? Remark;

            if (status == ProgressStatus.进行中 && ActualStartDate == null)
            {
                ActualStartDate = SystemTime.Now;
            }
            if (status == ProgressStatus.已完成 && ActualEndDate == null)
            {
                ActualEndDate = SystemTime.Now;
            }
        }
    }

    /// <summary>
    /// 进度状态
    /// </summary>
    public enum ProgressStatus
    {
        未开始 = 0,
        进行中 = 1,
        已完成 = 2,
        延迟 = 3
    }
}
```

#### 费用实体（BidExpense）

```csharp
// 文件：Domain/Models/DatabaseModel/BidExpense.cs
using Senparc.Ncf.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSBG.Xncf.BidManager
{
    /// <summary>
    /// 标书费用实体
    /// </summary>
    [Table(Register.DATABASE_PREFIX + nameof(BidExpense))]
    [Serializable]
    public class BidExpense : EntityBase<int>
    {
        /// <summary>
        /// 关联标书ID
        /// </summary>
        public int BidId { get; private set; }

        /// <summary>
        /// 费用项目
        /// </summary>
        [MaxLength(200)]
        public string ExpenseItem { get; private set; }

        /// <summary>
        /// 费用类别
        /// </summary>
        public ExpenseCategory Category { get; private set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; private set; }

        /// <summary>
        /// 发生日期
        /// </summary>
        public DateTime ExpenseDate { get; private set; }

        /// <summary>
        /// 收款方/经办人
        /// </summary>
        [MaxLength(100)]
        public string Payee { get; private set; }

        /// <summary>
        /// 发票号
        /// </summary>
        [MaxLength(100)]
        public string InvoiceNo { get; private set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Remark { get; private set; }

        /// <summary>
        /// 凭证附件路径
        /// </summary>
        [MaxLength(500)]
        public string AttachmentPath { get; private set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; private set; }

        // 导航属性
        public Bid Bid { get; set; }

        private BidExpense() { }

        public BidExpense(int bidId, string expenseItem, ExpenseCategory category,
            decimal amount, DateTime expenseDate, string payee)
        {
            BidId = bidId;
            ExpenseItem = expenseItem;
            Category = category;
            Amount = amount;
            ExpenseDate = expenseDate;
            Payee = payee;
            AddTime = SystemTime.Now;
            UpdateTime = SystemTime.Now;
        }

        public void Update(string expenseItem, ExpenseCategory category,
            decimal amount, DateTime expenseDate, string payee,
            string invoiceNo, string remark)
        {
            ExpenseItem = expenseItem ?? ExpenseItem;
            Category = category;
            Amount = amount;
            ExpenseDate = expenseDate;
            Payee = payee ?? Payee;
            InvoiceNo = invoiceNo ?? InvoiceNo;
            Remark = remark ?? Remark;
            UpdateTime = SystemTime.Now;
        }
    }

    /// <summary>
    /// 费用类别
    /// </summary>
    public enum ExpenseCategory
    {
        标书购买费 = 0,
        差旅费 = 1,
        打印装订费 = 2,
        快递物流费 = 3,
        专家咨询费 = 4,
        外包服务费 = 5,
        保证金 = 6,
        其他 = 7
    }
}
```

#### 人员实体（Personnel）

```csharp
// 文件：Domain/Models/DatabaseModel/Personnel.cs
using Senparc.Ncf.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSBG.Xncf.BidManager
{
    /// <summary>
    /// 人员实体
    /// </summary>
    [Table(Register.DATABASE_PREFIX + nameof(Personnel))]
    [Serializable]
    public class Personnel : EntityBase<int>
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(50)]
        public string Name { get; private set; }

        /// <summary>
        /// 工号
        /// </summary>
        [MaxLength(50)]
        public string EmployeeId { get; private set; }

        /// <summary>
        /// 部门
        /// </summary>
        [MaxLength(100)]
        public string Department { get; private set; }

        /// <summary>
        /// 角色
        /// </summary>
        [MaxLength(50)]
        public string Role { get; private set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [MaxLength(30)]
        public string Phone { get; private set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(100)]
        public string Email { get; private set; }

        /// <summary>
        /// 是否在职
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        public DateTime JoinTime { get; private set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; private set; }

        // 导航属性
        public ICollection<BidPersonnel> BidPersonnel { get; set; }

        private Personnel() { }

        public Personnel(string name, string employeeId, string department, string role, string phone)
        {
            Name = name;
            EmployeeId = employeeId;
            Department = department;
            Role = role;
            Phone = phone;
            IsActive = true;
            JoinTime = SystemTime.Now;
            AddTime = SystemTime.Now;
            UpdateTime = SystemTime.Now;
        }

        public void Update(string name, string department, string role,
            string phone, string email, bool isActive)
        {
            Name = name ?? Name;
            Department = department ?? Department;
            Role = role ?? Role;
            Phone = phone ?? Phone;
            Email = email ?? Email;
            IsActive = isActive;
            UpdateTime = SystemTime.Now;
        }
    }

    /// <summary>
    /// 标书-人员关联实体（多对多）
    /// </summary>
    [Table(Register.DATABASE_PREFIX + nameof(BidPersonnel))]
    [Serializable]
    public class BidPersonnel : EntityBase<int>
    {
        /// <summary>
        /// 标书ID
        /// </summary>
        public int BidId { get; private set; }

        /// <summary>
        /// 人员ID
        /// </summary>
        public int PersonnelId { get; private set; }

        /// <summary>
        /// 在本标书中的角色
        /// </summary>
        [MaxLength(50)]
        public string RoleInBid { get; private set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        [MaxLength(500)]
        public string TaskDescription { get; private set; }

        // 导航属性
        public Bid Bid { get; set; }
        public Personnel Personnel { get; set; }

        private BidPersonnel() { }

        public BidPersonnel(int bidId, int personnelId, string roleInBid, string taskDescription)
        {
            BidId = bidId;
            PersonnelId = personnelId;
            RoleInBid = roleInBid;
            TaskDescription = taskDescription;
            AddTime = SystemTime.Now;
        }
    }
}
```

#### 提醒实体（Reminder）

```csharp
// 文件：Domain/Models/DatabaseModel/Reminder.cs
using Senparc.Ncf.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSBG.Xncf.BidManager
{
    /// <summary>
    /// 提醒事项实体
    /// </summary>
    [Table(Register.DATABASE_PREFIX + nameof(Reminder))]
    [Serializable]
    public class Reminder : EntityBase<int>
    {
        /// <summary>
        /// 提醒类型
        /// </summary>
        public ReminderType Type { get; private set; }

        /// <summary>
        /// 提醒标题
        /// </summary>
        [MaxLength(200)]
        public string Title { get; private set; }

        /// <summary>
        /// 提醒内容
        /// </summary>
        [MaxLength(1000)]
        public string Content { get; private set; }

        /// <summary>
        /// 关联业务ID
        /// </summary>
        public int? RelatedId { get; private set; }

        /// <summary>
        /// 关联类型（Project/Bid/BidProgress）
        /// </summary>
        [MaxLength(50)]
        public string RelatedType { get; private set; }

        /// <summary>
        /// 紧急程度
        /// </summary>
        public ReminderLevel Level { get; private set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { get; private set; }

        /// <summary>
        /// 是否已处理
        /// </summary>
        public bool IsHandled { get; private set; }

        /// <summary>
        /// 截止时间
        /// </summary>
        public DateTime? DueTime { get; private set; }

        /// <summary>
        /// 指定处理人
        /// </summary>
        [MaxLength(50)]
        public string Assignee { get; private set; }

        private Reminder() { }

        public Reminder(ReminderType type, string title, string content,
            ReminderLevel level = ReminderLevel.一般, string relatedType = null,
            int? relatedId = null, DateTime? dueTime = null, string assignee = null)
        {
            Type = type;
            Title = title;
            Content = content;
            Level = level;
            RelatedType = relatedType;
            RelatedId = relatedId;
            DueTime = dueTime;
            Assignee = assignee;
            IsRead = false;
            IsHandled = false;
            AddTime = SystemTime.Now;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }

        public void MarkAsHandled()
        {
            IsHandled = true;
            IsRead = true;
        }
    }

    public enum ReminderType
    {
        投标截止 = 0,
        进度延期 = 1,
        标书审核 = 2,
        费用报销 = 3,
        保证金到期 = 4,
        自定义 = 99
    }

    public enum ReminderLevel
    {
        一般 = 0,
        重要 = 1,
        紧急 = 2
    }
}
```

#### 系统配置实体（SystemConfig）

```csharp
// 文件：Domain/Models/DatabaseModel/SystemConfig.cs
using Senparc.Ncf.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSBG.Xncf.BidManager
{
    /// <summary>
    /// 系统配置实体
    /// </summary>
    [Table(Register.DATABASE_PREFIX + nameof(SystemConfig))]
    [Serializable]
    public class SystemConfig : EntityBase<int>
    {
        /// <summary>
        /// 配置键
        /// </summary>
        [MaxLength(100)]
        public string ConfigKey { get; set; }

        /// <summary>
        /// 配置值
        /// </summary>
        [MaxLength(2000)]
        public string ConfigValue { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
```

### 5.3 DbContext 定义（遵循官方 XncfDatabaseDbContext 模式）

```csharp
// 文件：Domain/Models/DatabaseModel/BidManagerSenparcEntities.cs
using Microsoft.EntityFrameworkCore;
using Senparc.Ncf.Database;
using Senparc.Ncf.Core.Models;
using Senparc.Ncf.XncfBase.Database;

namespace HSBG.Xncf.BidManager.Models
{
    public class BidManagerSenparcEntities : XncfDatabaseDbContext
    {
        public BidManagerSenparcEntities(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<BidProgress> BidProgresses { get; set; }
        public DbSet<BidExpense> BidExpenses { get; set; }
        public DbSet<Personnel> Personnels { get; set; }
        public DbSet<BidPersonnel> BidPersonnel { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<SystemConfig> SystemConfigs { get; set; }

        //DOT REMOVE OR MODIFY THIS LINE 请勿移除或修改本行 - Entities Point
    }
}
```

### 5.4 Mapping 配置（遵循 [XncfAutoConfigurationMapping] 模式）

```csharp
// 示例：Domain/Models/DatabaseModel/Mapping/BidManager_ProjectConfigurationMapping.cs
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Senparc.Ncf.Core.Models.DataBaseModel;
using Senparc.Ncf.XncfBase.Attributes;

namespace HSBG.Xncf.BidManager.Models
{
    [XncfAutoConfigurationMapping]
    public class BidManager_ProjectConfigurationMapping : ConfigurationMappingWithIdBase<Project, int>
    {
        public override void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.Property(e => e.ProjectName).IsRequired();
            builder.Property(e => e.ProjectCode).IsRequired();
            builder.HasOne(e => e.Customer)
                   .WithMany(e => e.Projects)
                   .HasForeignKey(e => e.CustomerId);
        }
    }
}
```

> 所有 9 个实体的 Mapping 文件遵循同样模式：
> `BidManager_{Entity}ConfigurationMapping : ConfigurationMappingWithIdBase<{Entity}, int>`
> 标记 `[XncfAutoConfigurationMapping]` 实现自动发现。

---

## 六、应用服务层设计（遵循 AppServiceBase + [ApiBind] 模式）

### 6.1 标书 AppService（完整示例）

```csharp
// 文件：OHS/Local/AppService/BidAppService.cs
using Senparc.Ncf.Core.AppServices;
using HSBG.Xncf.BidManager.Domain.Services;
using HSBG.Xncf.BidManager.Models.DatabaseModel.Dto;
using HSBG.Xncf.BidManager.OHS.Local.PL;
using System;
using System.Threading.Tasks;
using Senparc.CO2NET.WebApi;
using Senparc.CO2NET;
using Senparc.Ncf.Utility;

namespace HSBG.Xncf.BidManager.OHS.Local.AppService
{
    public class BidAppService : AppServiceBase
    {
        private readonly BidService _bidService;

        public BidAppService(IServiceProvider serviceProvider, BidService bidService)
            : base(serviceProvider)
        {
            _bidService = bidService;
        }

        /// <summary>
        /// 获取标书列表
        /// </summary>
        [ApiBind]
        public async Task<AppResponseBase<List<BidDto>>> GetList(int projectId, int pageIndex = 1, int pageSize = 20)
        {
            return await this.GetResponseAsync<List<BidDto>>(async (response, logger) =>
            {
                var dt1 = SystemTime.Now;
                var list = await _bidService.GetListAsync(projectId, pageIndex, pageSize);
                var costMs = SystemTime.DiffTotalMS(dt1);
                logger.Append($"耗时：{costMs}ms");
                return list;
            });
        }

        /// <summary>
        /// 获取标书详情
        /// </summary>
        [ApiBind]
        public async Task<AppResponseBase<BidDto>> GetDetail(int id)
        {
            return await this.GetResponseAsync<BidDto>(async (response, logger) =>
            {
                var bid = await _bidService.GetDetailAsync(id);
                return bid;
            });
        }

        /// <summary>
        /// 创建标书
        /// </summary>
        [ApiBind(ApiRequestMethod = ApiRequestMethod.Post)]
        public async Task<StringAppResponse> Create(Bid_CreateRequest request)
        {
            return await this.GetStringResponseAsync(async (response, logger) =>
            {
                await _bidService.CreateAsync(request.ProjectId, request.BidCode,
                    request.BidName, request.PlanStartDate, request.PlanEndDate);
                response.StateCode = 200;
            });
        }

        /// <summary>
        /// 更新标书
        /// </summary>
        [ApiBind(ApiRequestMethod = ApiRequestMethod.Post)]
        public async Task<StringAppResponse> Update(Bid_UpdateRequest request)
        {
            return await this.GetStringResponseAsync(async (response, logger) =>
            {
                await _bidService.UpdateAsync(request.Id, request.BidName,
                    request.PlanStartDate, request.PlanEndDate,
                    request.ContentDesc, request.FilePath);
                response.StateCode = 200;
            });
        }

        /// <summary>
        /// 删除标书
        /// </summary>
        [ApiBind(ApiRequestMethod = ApiRequestMethod.Post)]
        public async Task<StringAppResponse> Delete(int id)
        {
            return await this.GetStringResponseAsync(async (response, logger) =>
            {
                await _bidService.DeleteAsync(id);
                response.StateCode = 200;
            });
        }

        /// <summary>
        /// 变更标书状态
        /// </summary>
        [ApiBind(ApiRequestMethod = ApiRequestMethod.Post)]
        public async Task<StringAppResponse> ChangeStatus(Bid_ChangeStatusRequest request)
        {
            return await this.GetStringResponseAsync(async (response, logger) =>
            {
                await _bidService.ChangeStatusAsync(request.Id, request.Status);
                response.StateCode = 200;
            });
        }
    }
}
```

### 6.2 PL 请求/响应对象（遵循官方模式）

```csharp
// 文件：OHS/Local/PL/BidRequest.cs
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HSBG.Xncf.BidManager.OHS.Local.PL
{
    /// <summary>
    /// 创建标书请求
    /// </summary>
    public class Bid_CreateRequest
    {
        [Required]
        [Description("项目ID")]
        public int ProjectId { get; set; }

        [Required]
        [MaxLength(50)]
        [Description("标书编号")]
        public string BidCode { get; set; }

        [Required]
        [MaxLength(200)]
        [Description("标书名称")]
        public string BidName { get; set; }

        [Required]
        [Description("计划开始日期")]
        public DateTime PlanStartDate { get; set; }

        [Required]
        [Description("计划完成日期")]
        public DateTime PlanEndDate { get; set; }
    }

    /// <summary>
    /// 更新标书请求
    /// </summary>
    public class Bid_UpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [MaxLength(200)]
        public string BidName { get; set; }

        public DateTime PlanStartDate { get; set; }

        public DateTime PlanEndDate { get; set; }

        [MaxLength(2000)]
        public string ContentDesc { get; set; }

        [MaxLength(500)]
        public string FilePath { get; set; }
    }

    /// <summary>
    /// 变更标书状态请求
    /// </summary>
    public class Bid_ChangeStatusRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Description("标书状态")]
        public BidStatus Status { get; set; }
    }
}
```

### 6.3 动态 API 端点一览

所有 AppService 方法通过 `[ApiBind]` 自动生成 API，路径格式为：

```
/api/{ModuleName}/{AppServiceName}/{MethodName}
```

| 功能 | 方法 | API 路径 |
|------|------|----------|
| 客户列表 | GET | `/api/HSBG.BidManager/CustomerAppService/GetList` |
| 客户详情 | GET | `/api/HSBG.BidManager/CustomerAppService/GetDetail` |
| 创建客户 | POST | `/api/HSBG.BidManager/CustomerAppService/Create` |
| 项目管理 | GET | `/api/HSBG.BidManager/ProjectAppService/GetList` |
| 项目状态变更 | POST | `/api/HSBG.BidManager/ProjectAppService/ChangeStatus` |
| 标书 CRUD | GET/POST | `/api/HSBG.BidManager/BidAppService/{Action}` |
| 进度列表 | GET | `/api/HSBG.BidManager/BidProgressAppService/GetList` |
| 进度状态更新 | POST | `/api/HSBG.BidManager/BidProgressAppService/UpdateStatus` |
| 费用统计 | GET | `/api/HSBG.BidManager/ExpenseAppService/GetStatistics` |
| 仪表盘数据 | GET | `/api/HSBG.BidManager/DashboardAppService/GetDashboard` |
| 未读提醒数 | GET | `/api/HSBG.BidManager/ReminderAppService/GetUnreadCount` |
| 统计报表 | GET | `/api/HSBG.BidManager/DashboardAppService/GetReport` |

---

## 七、领域服务层设计（继承 ServiceBase<T>）

```csharp
// 示例：Domain/Services/BidService.cs
using HSBG.Xncf.BidManager.Models.DatabaseModel.Dto;
using Senparc.Ncf.Core.Enums;
using Senparc.Ncf.Repository;
using Senparc.Ncf.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSBG.Xncf.BidManager.Domain.Services
{
    public class BidService : ServiceBase<Bid>
    {
        public BidService(IRepositoryBase<Bid> repo, IServiceProvider serviceProvider)
            : base(repo, serviceProvider)
        {
        }

        /// <summary>
        /// 获取标书分页列表
        /// </summary>
        public async Task<List<BidDto>> GetListAsync(int projectId, int pageIndex, int pageSize)
        {
            var query = projectId > 0
                ? base.GetFullList(z => z.ProjectId == projectId, z => z.Id, OrderingType.Descending)
                : base.GetFullList(z => true, z => z.Id, OrderingType.Descending);

            var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return base.Mapper.Map<List<BidDto>>(list);
        }

        /// <summary>
        /// 获取标书详情
        /// </summary>
        public async Task<BidDto> GetDetailAsync(int id)
        {
            var bid = await base.GetObjectAsync(z => z.Id == id);
            return base.Mapper.Map<BidDto>(bid);
        }

        /// <summary>
        /// 创建标书
        /// </summary>
        public async Task<BidDto> CreateAsync(int projectId, string bidCode,
            string bidName, DateTime planStartDate, DateTime planEndDate)
        {
            var bid = new Bid(projectId, bidCode, bidName, planStartDate, planEndDate);
            await base.SaveObjectAsync(bid).ConfigureAwait(false);
            return base.Mapper.Map<BidDto>(bid);
        }

        /// <summary>
        /// 更新标书信息
        /// </summary>
        public async Task<BidDto> UpdateAsync(int id, string bidName,
            DateTime planStartDate, DateTime planEndDate,
            string contentDesc, string filePath)
        {
            var bid = await base.GetObjectAsync(z => z.Id == id);
            bid.Update(bidName, planStartDate, planEndDate, contentDesc, filePath);
            await base.SaveObjectAsync(bid).ConfigureAwait(false);
            return base.Mapper.Map<BidDto>(bid);
        }

        /// <summary>
        /// 变更标书状态
        /// </summary>
        public async Task<BidDto> ChangeStatusAsync(int id, BidStatus status)
        {
            var bid = await base.GetObjectAsync(z => z.Id == id);
            bid.ChangeStatus(status);
            await base.SaveObjectAsync(bid).ConfigureAwait(false);
            return base.Mapper.Map<BidDto>(bid);
        }

        /// <summary>
        /// 删除标书（逻辑删除）
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var bid = await base.GetObjectAsync(z => z.Id == id);
            bid.Flag = true;
            await base.SaveObjectAsync(bid).ConfigureAwait(false);
        }
    }
}
```

---

## 八、功能模块详细设计

### 8.1 首页仪表盘

**功能定位**：展示各项目关键指标的聚合视图，提供快捷入口

**展示内容**：

| 指标 | 数据来源 | 展示形式 |
|------|----------|----------|
| 进行中项目数 | Project.Status = 制作中 | `el-statistic` 卡片 |
| 本月标书数 | Bid.AddTime 当月 | `el-statistic` 卡片 |
| 待处理提醒数 | Reminder.IsHandled = false | `el-statistic` 卡片 + 角标 |
| 本月费用总额 | BidExpense 当月汇总 | `el-statistic` 卡片 |
| 中标率 | 已中标 / 已结项总数 | 环形图（ECharts） |
| 近期项目时间线 | Project 按更新时间排序 | `el-timeline` |
| 费用月度趋势 | BidExpense 按月汇总 | 折线图（ECharts） |
| 标书状态分布 | Bid.Status 统计 | 饼图（ECharts） |
| 快捷入口 | — | `el-card` 点击跳转 |

**技术实现**：
- 页面：`Areas/Admin/Pages/BidManager/Index.cshtml`（继承 `AdminXncfModulePageModelBase`）
- API：`DashboardAppService.GetDashboard()` — 一次请求返回所有聚合数据
- 使用 `ECharts` 渲染图表，在 `scripts` section 中初始化

### 8.2 客户管理

| 功能 | 说明 |
|------|------|
| 客户列表 | 分页、搜索（名称/电话）、按类型/活跃状态筛选 |
| 客户详情 | 侧边栏抽屉或独立页面，展示客户基本信息 |
| 新增/编辑客户 | 表单弹窗，必填项：名称、联系人、电话 |
| 关联项目列表 | 客户详情中展示该客户所有投标项目 |
| 导入/导出 | Excel 批量导入导出（使用 EPPlus 或 ClosedXML） |

### 8.3 项目管理

| 功能 | 说明 |
|------|------|
| 项目列表 | 分页、多条件筛选（状态/类型/优先级/客户）、搜索 |
| 项目详情 | 含客户信息、标书列表、费用汇总 |
| 新增/编辑项目 | 客户下拉选择（支持搜索过滤）、日期选择器 |
| 状态流转 | 下拉按钮或步骤条，限制合法状态转换 |
| 统计分析 | 项目状态统计饼图、月度投标走势图 |

**状态流转规则**：

```
跟踪中 → 已立项 → 制作中 → 已投标 → 已中标
                                 → 未中标
任意非终态 → 已废标
```

### 8.4 标书管理

| 功能 | 说明 |
|------|------|
| 标书列表 | 按项目筛选、状态筛选、关键字搜索 |
| 标书详情 | 含进度时间线、费用列表、参与人员 |
| 新增/编辑标书 | 自动带入项目信息，设置计划日期 |
| 文件上传 | 标书文件上传（支持多文件），记录版本 |
| 审核流转 | 编制中→审核中→审核通过/审核退回 |
| 费用汇总 | 自动汇总该标书下所有费用 |

### 8.5 制作进度

| 功能 | 说明 |
|------|------|
| 进度看板 | 按"未开始/进行中/已完成/延迟"分列展示（拖拽排序） |
| 甘特图 | 展示所有进行中的标书时间线（使用 Gantt 库或 ECharts） |
| 阶段更新 | 状态下拉 + 百分比滑块 + 备注 |
| 延期预警 | 超出计划完成日期自动置为"延迟"状态 |
| 进度模板 | 新建标书时根据预设模板自动生成阶段节点 |

**预设阶段模板**（由 `SystemConfig.ProgressTemplate` 配置）：

```
大纲编写 → 初稿撰写 → 内部审核 → 修改完善 → 定稿 → 排版装订 → 最终审核 → 递交
```

### 8.6 费用管理

| 功能 | 说明 |
|------|------|
| 费用列表 | 按标书/类别/日期范围查询 |
| 费用录入 | 表单录入，支持上传发票等凭证附件 |
| 费用编辑/删除 | 修改金额、类别等信息 |
| 费用汇总 | 按标书维度汇总，按类别/月份统计 |
| 费用图表 | 饼图（类别占比）、柱状图（月度对比）、对标书预算 |
| 导出 | Excel 导出费用明细 |

### 8.7 人员管理

| 功能 | 说明 |
|------|------|
| 人员列表 | 按部门/角色/在职状态筛选 |
| 人员详情 | CRUD 操作 |
| 任务分配 | 为标书添加参与人员，指定角色和任务 |
| 工作负载 | 展示每人当前参与标书数、进行中任务数 |

**预设角色**：项目经理、标书专员、技术专家、审核员、装订员

### 8.8 提醒中心

| 功能 | 说明 |
|------|------|
| 提醒列表 | 已读/未读、类型、紧急程度筛选 |
| 提醒详情 | 查看内容，关联跳转到业务页面 |
| 已读/处理 | 标记已读、标记已处理 |
| 自动生成 | 后台扫描自动创建（见下方策略） |
| 自定义提醒 | 手动创建提醒项 |
| 角标通知 | 所有页面顶部显示未处理提醒数 |

**提醒自动生成策略**：

| 场景 | 触发条件 | 提醒类型 | 紧急程度 |
|------|----------|----------|----------|
| 投标截止 | Bid.Deadline 前 3 天 | 投标截止 | 紧急 |
| 进度延期 | BidProgress 超 PlanEndDate 且未完成 | 进度延期 | 重要 |
| 标书审核 | Bid 进入审核中状态 | 标书审核 | 一般 |
| 保证金到期 | 费用类别为保证金且到期前 7 天 | 保证金到期 | 重要 |

> 建议通过后台定时任务（Quartz.NET 或 HostedService）每日扫描执行。

### 8.9 统计中心

**功能定位**：多维度的数据统计分析与可视化报表

| 统计维度 | 分析指标 | 图表类型 |
|----------|----------|----------|
| 项目统计 | 按状态分布、按类型分布、月度投标走势 | 饼图 + 折线图 |
| 标书统计 | 按状态分布、月度制作量 | 柱状图 |
| 中标分析 | 中标率趋势、按客户/类型分布 | 折线图 + 饼图 |
| 费用统计 | 按类别汇总、月度趋势、人均费用 | 饼图 + 柱状图 |
| 人员绩效 | 个人完成标书数、按时完成率 | 排行榜表格 |
| 客户分析 | 投标次数排行、中标率排行 | 排行榜表格 + 柱状图 |
| 年度报表 | 年度综合汇总（项目数、中标率、费用总额） | 综合面板 |

**实现要点**：
- API：`DashboardAppService.GetReport(type, startDate, endDate)` 支持按时间范围和报表类型查询
- 前端使用 ECharts 渲染全部图表
- 支持时间范围选择器（本月/本季/本年/自定义）
- 预留导出 PDF / Excel 能力

### 8.10 系统设置

| 配置项 | 键 | 默认值 | 说明 |
|--------|-----|--------|------|
| 投标截止提醒天数 | `RemindDaysBeforeBidDeadline` | 3 | 投标截止前 N 天提醒 |
| 项目编号前缀 | `ProjectCodePrefix` | `PRJ-` | 自动编号前缀 |
| 标书编号前缀 | `BidCodePrefix` | `BID-` | 自动编号前缀 |
| 进度阶段模板 | `ProgressTemplate` | 8段预设 | 新建标书时自动生成 |
| 默认预算 | `DefaultBudget` | 0 | 标书默认预算金额 |
| 是否启用邮件通知 | `EnableEmailNotify` | false | 提醒是否同步邮件 |

---

## 九、前端页面规范（遵循官方 Vue.js + Element UI 模式）

### 9.1 页面模板（以标书列表为例）

```html
@* Areas/Admin/Pages/BidManager/Bid/List.cshtml *@
@page
@model HSBG.Xncf.BidManager.Areas.BidManager.Pages.Bid.List
@{
    ViewData["Title"] = "标书管理";
    Layout = "_Layout_Vue";
}

@section breadcrumbs{
    <el-breadcrumb-item>标书管理</el-breadcrumb-item>
    <el-breadcrumb-item>标书列表</el-breadcrumb-item>
}

@section HeaderContent{
    <style>
        .bid-list .toolbar { margin-bottom: 20px; display: flex; justify-content: space-between; }
    </style>
}

<div class="bid-list">
    <el-container>
        <el-main>
            <!-- 工具栏 -->
            <div class="toolbar">
                <div>
                    <el-select v-model="filterProjectId" placeholder="按项目筛选" clearable>
                        <el-option v-for="p in projects"
                            :key="p.id" :label="p.projectName" :value="p.id">
                        </el-option>
                    </el-select>
                    <el-input v-model="keyword" placeholder="搜索标书名称/编号"
                        style="width: 250px; margin-left: 10px;" clearable></el-input>
                    <el-button type="primary" icon="el-icon-search" style="margin-left: 10px;"
                        @@click="loadData">查询</el-button>
                </div>
                <el-button type="success" icon="el-icon-plus" @@click="showCreateDialog">新建标书</el-button>
            </div>

            <!-- 数据表格 -->
            <el-table :data="tableData" border stripe v-loading="loading">
                <el-table-column prop="bidCode" label="标书编号" width="120"></el-table-column>
                <el-table-column prop="bidName" label="标书名称"></el-table-column>
                <el-table-column label="状态" width="100">
                    <template slot-scope="scope">
                        <el-tag :type="statusType(scope.row.status)">{{ scope.row.status }}</el-tag>
                    </template>
                </el-table-column>
                <el-table-column prop="version" label="版本" width="80"></el-table-column>
                <el-table-column label="计划时间" width="180">
                    <template slot-scope="scope">
                        {{ scope.row.planStartDate }} ~ {{ scope.row.planEndDate }}
                    </template>
                </el-table-column>
                <el-table-column label="操作" width="200">
                    <template slot-scope="scope">
                        <el-button size="mini" @@click="editRow(scope.row)">编辑</el-button>
                        <el-button size="mini" type="danger" @@click="deleteRow(scope.row)">删除</el-button>
                    </template>
                </el-table-column>
            </el-table>

            <!-- 分页 -->
            <el-pagination background layout="total, prev, pager, next"
                :total="total" :page-size="pageSize" :current-page.sync="pageIndex"
                @@current-change="loadData" style="margin-top: 20px; text-align: right;">
            </el-pagination>
        </el-main>
    </el-container>
</div>

@section scripts{
    <script>
        var app = new Vue({
            el: "#app",
            data() {
                return {
                    tableData: [],
                    projects: [],
                    filterProjectId: null,
                    keyword: '',
                    pageIndex: 1,
                    pageSize: 20,
                    total: 0,
                    loading: false
                };
            },
            mounted() {
                this.loadProjects();
                this.loadData();
            },
            methods: {
                async loadData() {
                    this.loading = true;
                    const res = await service.get('/api/HSBG.BidManager/BidAppService/GetList', {
                        params: {
                            projectId: this.filterProjectId || 0,
                            keyword: this.keyword,
                            pageIndex: this.pageIndex,
                            pageSize: this.pageSize
                        }
                    });
                    this.tableData = res.data.data || [];
                    this.total = res.data.total || 0;
                    this.loading = false;
                },
                statusType(status) {
                    const map = { '编制中': '', '审核中': 'warning', '审核通过': 'success', '审核退回': 'danger', '已递交': 'info' };
                    return map[status] || '';
                }
                // ... 省略其他方法
            }
        });
    </script>
}
```

### 9.2 页面基类（PageModel）

```csharp
// Areas/Admin/Pages/BidManager/Bid/List.cshtml.cs
using Senparc.Ncf.Service;
using System;

namespace HSBG.Xncf.BidManager.Areas.BidManager.Pages.Bid
{
    public class List : Senparc.Ncf.AreaBase.Admin.AdminXncfModulePageModelBase
    {
        public List(Lazy<XncfModuleService> xncfModuleService) : base(xncfModuleService)
        {
        }

        public void OnGet()
        {
        }
    }
}
```

> 所有页面 PageModel 均继承 `AdminXncfModulePageModelBase`，这是 NCF 标准页面的基类。

---

## 十、数据库设计总览

### 10.1 全部表清单

| 表名 | 实体 | 主键 | 外键 | 说明 |
|------|------|------|------|------|
| `HSBG_BidManager_Customer` | Customer | Id | — | 客户信息 |
| `HSBG_BidManager_Project` | Project | Id | CustomerId | 投标项目 |
| `HSBG_BidManager_Bid` | Bid | Id | ProjectId | 标书 |
| `HSBG_BidManager_BidProgress` | BidProgress | Id | BidId | 制作进度 |
| `HSBG_BidManager_BidExpense` | BidExpense | Id | BidId | 费用记录 |
| `HSBG_BidManager_Personnel` | Personnel | Id | — | 人员信息 |
| `HSBG_BidManager_BidPersonnel` | BidPersonnel | Id | BidId, PersonnelId | 标书-人员关联 |
| `HSBG_BidManager_Reminder` | Reminder | Id | — | 提醒事项 |
| `HSBG_BidManager_SystemConfig` | SystemConfig | Id | — | 系统配置 |

### 10.2 所有实体继承 EntityBase<int>，自动获得以下字段：

```csharp
public int Id { get; set; }          // 主键
public DateTime AddTime { get; set; } // 创建时间
public bool Flag { get; set; }       // 软删除标记
```

---

## 十一、开发路线图

| 阶段 | 内容 | 产出物 |
|------|------|--------|
| **Phase 1** | 实体层搭建 | 全部 9 个 Entity + Dto + Mapping + DbContext |
| **Phase 2** | 领域层 + 应用层 | 全部 Service + AppService + PL |
| **Phase 3** | 客户管理 + 项目管理 | List/Detail 页面 + CRUD API |
| **Phase 4** | 标书管理 + 人员管理 | List/Detail 页面 + 多对多关联 |
| **Phase 5** | 制作进度 + 费用管理 | 看板/甘特图 + 统计图表 |
| **Phase 6** | 提醒中心 + 统计中心 + 仪表盘 | 自动提醒 + 报表 + 首页 |
| **Phase 7** | 系统设置 + 联调测试 | 配置页面 + 端到端测试 |

---

## 十二、编码规范约定

> 以下规范均提取自 NCF 官方模块（Senparc.Xncf.Accounts、Senparc.Xncf.Installer、Senparc.Areas.Admin）源码，确保 HSBG.Xncf.BidManager 与框架原生模块风格完全一致。

| 规范项 | 约定 |
|--------|------|
| **命名空间** | `HSBG.Xncf.BidManager` 为根，子目录按层级追加 |
| **实体** | 继承 `EntityBase<int>`，标记 `[Table(Register.DATABASE_PREFIX + nameof(X))]`、`[Serializable]` |
| **构造函数** | 私有无参构造 + 有参业务构造，属性 set 为 `private set` |
| **DTO** | 独立 POCO 类，放在 `Dto/` 目录 |
| **Mapping** | 继承 `ConfigurationMappingWithIdBase<T, int>`，标记 `[XncfAutoConfigurationMapping]` |
| **Service** | 继承 `ServiceBase<T>`，通过构造函数注入 `IRepositoryBase<T>` |
| **AppService** | 继承 `AppServiceBase`，方法标记 `[ApiBind]`，返回 `AppResponseBase<T>` 或 `StringAppResponse` |
| **PL** | 请求对象命名 `{Entity}_{Action}Request`，响应对象命名 `{Entity}_{Action}Response` |
| **PageModel** | 继承 `AdminXncfModulePageModelBase` |
| **Register** | `partial class` 拆分 3-4 个文件，实现 `IXncfRegister`、`IAreaRegister`、`IXncfDatabase`、`IXncfRazorRuntimeCompilation` |
| **注释** | 使用 `/// <summary>` XML 文档注释，`#region` 组织代码块 |
| **软删除** | 设置 `obj.Flag = true` 后 `SaveObject`，不物理删除 |
| **时间** | 使用 `Senparc.CO2NET.SystemTime.Now`（兼容单元测试） |
| **数据库前缀** | 常量 `DATABASE_PREFIX` 用于表名，确保全局不冲突 |
| **菜单图标** | 使用 Font Awesome（`fa fa-xxx`）或 Element Icon（`el-icon-xxx`） |
