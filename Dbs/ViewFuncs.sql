CREATE function [dbo].[函数更新_业务备案_进口票_滞箱费警示状态](@id uniqueidentifier)
returns @tb table (滞箱费警示状态 nvarchar(50))
as
begin
	declare @count int,@count1 int, @state nvarchar(50) 
	set @state = ''
--单证晚到5天或异常
	select @count = count(id) from 业务备案_进口票 where id = @id and (单证晚到 >= 5 or 异常情况 like '21,')
	if @count > 0
	begin
		set @state = @state + '1,'
	end
	
--用箱超期, 预计用箱天数>免箱天数
	select @count = count(id) from 业务备案_进口票 where id = @id and 预计用箱天数 > 免箱天数 
	if @count > 0
	begin
		set @state = @state + '2,'
	end
	
--车队还箱超期,	还箱时间>货代要求还箱时间止
	select @count = count(id) from 业务备案_进口箱 where 票 = @id and 还箱时间 > 货代还箱时间要求止
	select @count1 = count(id) from 业务备案_进口票 where id = @id and 承运标志 = 1 	
	if @count > 0 and @count1 > 0
	begin
		set @state = @state + '3,'
	end
	
--实际滞箱费产生, 有滞箱费费用
	select @count = count(id) from 财务_费用 where 费用实体 = @id and 费用项 = 167
	if @count > 0
	begin
		set @state = @state + '4,'
	end
	
--委托人承担未处理, 委托人无费用且未完全标志收	
	select @count = count(a.id) from 财务_费用 as a inner join 业务备案_普通票 as b on a.费用实体 = b.id and a.相关人 = b.委托人
	where a.费用实体 = @id and a.费用项 = 167
	select @count1 = count(id) from 财务_费用信息 where 票 = @id and 费用项 = 167 and submitted = 0
	if @count = 0 and @count1 > 0
	begin
		set @state = @state + '5,'
	end
	
--车队承担未处理, 车队无费用且未完全标志付
	select @count = count(a.id) from 财务_费用 as a inner join 业务备案_进口票 as b on a.费用实体 = b.id and a.相关人 = b.承运人
	where a.费用实体 = @id and a.费用项 = 167
	select @count1 = count(id) from 财务_费用信息 where 票 = @id and 费用项 = 167 and 完全标志付 = 0
	if @count = 0 and @count1 > 0
	begin
		set @state = @state + '6,'
	end
	
--退滞箱费未确认, 退滞箱费未凭证未对账
	select @count = count(a.id) FROM         财务_费用 AS a INNER JOIN
                      业务备案_普通票 AS B ON a.费用实体 = B.Id AND a.相关人 <> B.委托人 INNER JOIN
                      业务备案_进口票 AS D ON D.Id = B.Id AND a.相关人 <> D.承运人
    WHERE     a.费用实体 = @id and a.费用项 = 167 and a.对账单 is null and a.凭证费用明细 is null and ((收付标志 = 1 and 金额 > 0) or (收付标志 = 2 and 金额 < 0))
	if @count > 0
	begin
		set @state = @state + '7,'
	end

--委托人承担未确认, 问委托人收的正数费用未凭证未对账
	select @count = count(a.id) from 财务_费用 as a inner join 业务备案_普通票 as b on a.费用实体 = b.id and a.相关人 = b.委托人 	
	where  a.费用实体 = @id and a.费用项 = 167 and 对账单 is null and 凭证费用明细 is null and 收付标志 = 1 and 金额 > 0 
	if @count > 0
	begin
		set @state = @state + '8,'
	end

--车队承担未确认, 车队费用未凭证未对账
	select @count = count(a.id) from 财务_费用 as a inner join 业务备案_进口票 as b on a.费用实体 = b.id and a.相关人 = b.承运人
	where a.费用实体 = @id and a.费用项 = 167 and 对账单 is null and 凭证费用明细 is null
	if @count > 0
	begin
		set @state = @state + '9,'
	end
	
	insert into @tb values (@state)
	return 
end




CREATE function [dbo].[函数查询_业务费用小计_票费用项_已收](@id uniqueidentifier,@费用项 int,@收付标志 int)
returns table
as
return
(
	--已收金额
	--财务_费用.凭证 is not null
	select sum(金额) as 已收金额 from 财务_费用 
	where 凭证费用明细 is not null and 收付标志 = @收付标志 
	and 费用实体 = @id and 费用项 = @费用项 
)





CREATE function [dbo].[函数查询_业务费用小计_票费用项_应收](@id uniqueidentifier,@费用项 int,@收付标志 int)
returns table
as
return
(
	--应收金额
	--财务_费用.凭证 is not null or 对账单.submitted = 1
	select sum(a.金额) as 应收金额 from 财务_费用 as a 
	inner join 财务_对账单 as b on a.对账单 = b.id 
	where (a.对账单 is not null or b.submitted = 1) and a.收付标志 = @收付标志 
	and a.费用实体 = @id and a.费用项 = @费用项 
)





CREATE function [dbo].[函数查询_滞箱费减免进口箱](@票id varchar(50),@箱id varchar(50))
returns @tb table(费用实体 varchar(50),箱 varchar(50),委托人承担 decimal(18,2),车队承担 decimal(18,2),最终滞箱费 decimal(18,2),退滞箱费 decimal(18,2),无法归属 decimal(18,2),自己承担 decimal(18,2))
as
begin
declare @wtrcd decimal(18,2),@cdcd decimal(18,2),@zzzxf decimal(18,2),@tzxf decimal(18,2),@wfgs decimal(18,2),@zjcd decimal(18,2)
select @wtrcd = sum(委托人承担) from 视图信息_滞箱费减免进口箱_委托人承担 
where 费用实体 = @票id and 箱 = @箱id group by 费用实体,箱

select @cdcd = sum(车队承担) from 视图信息_滞箱费减免进口箱_车队承担 where 费用实体 = @票id and 箱 = @箱id

select @zzzxf = sum(最终滞箱费) from 视图信息_滞箱费减免进口箱_最终滞箱费 where 费用实体 = @票id and 箱 = @箱id

select @tzxf = sum(退滞箱费) from 视图信息_滞箱费减免进口箱_退滞箱费 where 费用实体 = @票id and 箱 = @箱id

select @wfgs = sum(无法归属) from 视图信息_滞箱费减免进口箱_无法归属 where 费用实体 = @票id and 箱 = @箱id

select @zjcd = (case when @zzzxf is null then 0 else @zzzxf end) - (case when @wtrcd is null then 0 else @wtrcd end) - (case when @cdcd is null then 0 else @cdcd end) - (case when @tzxf is null then 0 else @tzxf end)
insert into @tb values(@票id,@箱id,@wtrcd,@cdcd,@zzzxf,@tzxf,@wfgs,@zjcd)
return
end






CREATE function [dbo].[函数查询统计_借贷](@入账日期Ge datetime='2005-1-1',@入账日期Le datetime='2020-12-31')
returns table
as
return
(
	select 相关人,类型,
sum(case when 日期 < @入账日期Ge then isnull(金额,0) else 0 end)  as 月初值,
	sum(case when 日期 >= @入账日期Ge and 日期 <= @入账日期Le and 金额 > 0 then 金额 else 0 end) as 本月新增,
    sum(case when 日期 >= @入账日期Ge and 日期 <= @入账日期Le and 金额 < 0 then -金额 else 0 end) as 本月减少 ,
sum(case when 日期 <= @入账日期Le then isnull(金额,0) else 0 end) as 月末值
from 视图查询_借贷 
    group by 相关人,类型
	
)







CREATE function [dbo].[函数查询统计_押金](@入账日期Ge datetime='2005-1-1',@入账日期Le datetime='2020-12-31')
returns table
as
return
(
	select 相关人,类型,
sum(case when 日期 < @入账日期Ge then isnull(金额,0) else 0 end)  as 月初值,
	sum(case when 日期 >= @入账日期Ge and 日期 <= @入账日期Le and 金额 > 0 then 金额 else 0 end) as 本月新增,
    sum(case when 日期 >= @入账日期Ge and 日期 <= @入账日期Le and 金额 < 0 then -金额 else 0 end) as 本月减少 ,
sum(case when 日期 <= @入账日期Le then isnull(金额,0) else 0 end) as 月末值
from 视图查询_押金 
    group by 相关人,类型
	
)








CREATE function [dbo].[函数更新_财务_费用信息_费用承担](@费用信息id varchar(50))
returns @tb table(委托人承担 decimal(18,2),车队承担 decimal(18,2),对外付款 decimal(18,2),自己承担 decimal(18,2))
as
begin
declare @wtrcd decimal(18,2),@cdcd decimal(18,2),@dwfk decimal(18,2),@zjcd decimal(18,2) 
select @wtrcd = sum(委托人承担) from 视图信息_财务费用信息_委托人承担 
where 费用信息 = @费用信息id group by 费用信息,费用类别

select @cdcd = sum(车队承担) from 视图信息_财务费用信息_车队承担 where 费用信息 = @费用信息id 

select @dwfk = sum(对外付款) from 视图信息_财务费用信息_对外付款 where 费用信息 = @费用信息id 

select @zjcd = (case when @dwfk is null then 0 else @dwfk end) - (case when @wtrcd is null then 0 else @wtrcd end) - (case when @cdcd is null then 0 else @cdcd end) 
insert into @tb values(@wtrcd,@cdcd,@dwfk,@zjcd)
return
end




create function [dbo].[函数更新_财务_费用信息_车队承担](@费用信息id varchar(50))
returns table
as
return
(
	select sum(车队承担) as 车队承担 from 视图信息_财务费用信息_车队承担 where 费用信息 = @费用信息id 

)



CREATE function  [dbo].[函数更新_财务_费用信息_付款对账凭证状态](@id varchar(50))
returns table
as
return
(
select (case when count(case when b.收付标志 = 2 then 1 end) = 0 then null 
			 when count(case when b.收付标志 = 1 then 1 end) = 0 and a.Submitted = 'False' then 1 
			 when a.凭证号 is null and a.对账单号 is null then 2 
			 when a.凭证号 is not null or a.对账单号 is not null then 3 end) as 付款对账凭证状态
from 财务_费用信息 as a inner join 财务_费用 as b on a.id = b.费用信息 
where a.id = @id
group by a.Submitted,a.凭证号,a.对账单号

)






CREATE function [dbo].[函数查询统计_投资](@入账日期Ge datetime='2005-1-1',@入账日期Le datetime='2020-12-31')
returns table
as
return
(
	select 相关人,类型,
sum(case when 入账日期 < @入账日期Ge then isnull(投资金额,0) -isnull(撤资金额,0) else 0 end)  as 月初值,
	sum(case when 入账日期 >= @入账日期Ge and 入账日期 <= @入账日期Le then isnull(投资金额,0) else 0 end) as 本月新增,
    sum(case when 入账日期 >= @入账日期Ge and 入账日期 <= @入账日期Le then isnull(撤资金额,0) else 0 end) as 本月减少 ,
sum(case when 入账日期 <= @入账日期Le then isnull(投资金额,0) -isnull(撤资金额,0) else 0 end) as 月末值
from 视图查询_非业务费用明细_投资 
    group by 相关人,类型
	
)







CREATE function [dbo].[函数更新_财务_费用信息_对账单号](@id varchar(50))
returns table
as
return
(
select dbo.Concatenate(DISTINCT c.编号) as 对账单号
from 财务_费用 as a left join 财务_费用信息 as b on a.费用信息 = b.id
left join 财务_对账单 as c on a.对账单 = c.id
where b.id = @id
)


create function [dbo].[函数更新_财务_费用信息_对外付款](@费用信息id varchar(50))
returns table
as
return
(
	select sum(对外付款) as 对外付款 from 视图信息_财务费用信息_对外付款 where 费用信息 = @费用信息id 

)





CREATE function [dbo].[函数更新_财务_费用信息_付款时间](@id varchar(50))
returns table
as
return
(
select max(a.Updated) as 付款修改时间,max(a.Created) as 付款创建时间
from 财务_费用 as a left join 财务_费用信息 as b on a.费用信息 = b.id 
where b.id = @id and 收付标志 = 2 
)




CREATE function [dbo].[函数更新_财务_费用信息_凭证号](@id varchar(50))
returns table
as
return
(
select dbo.Concatenate(DISTINCT d.凭证号) as 凭证号
from 财务_费用 as a left join 财务_费用信息 as b on a.费用信息 = b.id
left join 财务_凭证费用明细 as c on a.凭证费用明细 = c.id
left join 财务_凭证 as d on c.凭证 = d.id
where b.id = @id
)




CREATE function  [dbo].[函数更新_财务_费用信息_收款对账凭证状态](@id varchar(50))
returns table
as
return
(
select (case when count(case when b.收付标志 = 1 then 1 end) = 0 then null  
			 when count(case when b.收付标志 = 2 then 1 end) = 0 and a.Submitted = 'False' then 1 
			 when a.凭证号 is null and a.对账单号 is null then 2 
			 when a.凭证号 is not null or a.对账单号 is not null then 3 end) as 收款对账凭证状态
from 财务_费用信息 as a inner join 财务_费用 as b on a.id = b.费用信息 
where a.id = @id
group by a.Submitted,a.凭证号,a.对账单号

)





CREATE function [dbo].[函数更新_财务_费用信息_收款时间](@id varchar(50))
returns table
as
return
(
select max(a.Updated) as 收款修改时间,max(a.Created) as 收款创建时间
from 财务_费用 as a left join 财务_费用信息 as b on a.费用信息 = b.id 
where b.id = @id and 收付标志 = 1 
)




CREATE function [dbo].[函数更新_财务_费用信息_填全状态](@id varchar(50))
returns table
as
return
(
select count(a.id) as 填全状态
from 财务_费用 as a left join 财务_费用信息 as b on a.费用信息 = b.id 
where b.id = @id and (相关人 is null or 金额 is null)
)



CREATE function [dbo].[函数更新_财务_费用信息_委托人承担](@费用信息id varchar(50))
returns table
as
return
(
	select sum(委托人承担) as 委托人承担 from 视图信息_财务费用信息_委托人承担 
where 费用信息 = @费用信息id group by 费用信息,费用类别

)



CREATE function [dbo].[函数更新_查询_票信息_进口] (@id uniqueidentifier)
RETURNS TABLE 
AS
RETURN 
(
	select Id,通关天数=(DATEDIFF(day,(case when 到港时间 is null or 单证齐全时间 is null then null
														 when 到港时间 >= 单证齐全时间 then 到港时间
														  when 到港时间 < 单证齐全时间 then 单证齐全时间 end),放行时间)) from 查询_票信息_进口 where id = @id

)










CREATE function [dbo].[函数更新_业务备案_进口票](@id uniqueidentifier)
returns @tb table(单证晚到 int,通关天数 int,结束时间 datetime,承运超期 int,更新时间 datetime,用箱天数 int)
as
begin
declare @dzwd int,@tgts int,@jsTime datetime,@cycq int,@gxTime datetime,@yxts int,@还箱情况 int
--单证晚到
select @dzwd = (case when datediff(day,到港时间,单证齐全时间)+1 > 0 then datediff(day,到港时间,单证齐全时间)+1 else 0 end) from 业务备案_进口票 where id = @id

--通关天数
select @tgts=(DATEDIFF(day,(case when 到港时间 is null or 单证齐全时间 is null then null
														 when 到港时间 >= 单证齐全时间 then 到港时间
														  when 到港时间 < 单证齐全时间 then 单证齐全时间 end),放行时间)+1) from 业务备案_进口票 where id = @id

--结束时间，最后一个还箱时间。   没还清，当前时间
select @jsTime = case when b.承运标志 = 0 then b.放行时间  when b.承运标志 = 1 and c.箱量 = count(a.id) then max(a.还箱时间) else null end
from 业务备案_进口箱 as a inner join 业务备案_进口票 as b on a.票 = b.id inner join 业务备案_普通票 as c on c.Id = b.id
where a.票 = @id
group by b.承运标志,b.放行时间,c.箱量


--承运超期
select @cycq = (case when datediff(day,放行时间,isnull(@jsTime,getdate())) > 要求承运天数 then datediff(day,dateadd(dd,要求承运天数,放行时间),@jsTime)
					else null end) from 业务备案_进口票 where id = @id and 承运标志 = 'true'

--更新时间
select @gxTime = getdate()

--用箱天数
select @yxts = datediff(day,到港时间,@jsTime) from 业务备案_进口票 where id = @id

insert into @tb values (@dzwd,@tgts,@jsTime,@cycq,@gxTime,@yxts)
return 
end






create function [dbo].[函数更新_财务_费用信息_警示状态](@id varchar(50))
returns table
as
return
(
select *--(case when Submitted = 'False' and ())
from 财务_费用信息 
where id = '77bd577d-9db6-4001-9bd1-9ca40168b84f'
--
--
--datediff(day,(case when 付款创建时间 < 收款创建时间 then 收款创建时间 when 收款创建时间 < 付款创建时间 then 付款创建时间 end),getdate()) <= 2
--or
--datediff(day,(case when 付款修改时间 < 收款修改时间 then 收款修改时间 when 收款修改时间 < 付款修改时间 then 付款修改时间 end),getdate()) <= 2
--or
--select datediff(day,(
--select 凭证时间
--from 函数更新_财务_费用信息_凭证号('77bd577d-9db6-4001-9bd1-9ca40168b84f')),getdate()) <= 2

)

create function [dbo].[函数更新_业务备案_进口票_承运超期](@id uniqueidentifier)
returns table
as
return
(
	select (case when datediff(day,放行时间,isnull(结束时间,getdate())) > 要求承运天数 then datediff(day,dateadd(dd,要求承运天数,放行时间),结束时间)
					else null end) as 承运超期
	from 业务备案_进口票 where id = @id and 承运标志 = 1

)


create function [dbo].[函数更新_业务备案_进口票_单证晚到](@id uniqueidentifier)
returns table
as
return
(
	select (case when datediff(day,到港时间,单证齐全时间)+1 > 0 then datediff(day,到港时间,单证齐全时间)+1 else 0 end) as 单证晚到
	from 业务备案_进口票 where id = @id

)


CREATE function [dbo].[函数更新_业务备案_进口票_结束时间](@id uniqueidentifier)
returns table
as
return
(
	select (case when b.承运标志 = 0 then b.放行时间  when b.承运标志 = 1 and c.箱量 = count(a.id) then max(a.还箱时间) else null end) as 结束时间
	from 业务备案_进口箱 as a inner join 业务备案_进口票 as b on a.票 = b.id inner join 业务备案_普通票 as c on c.Id = b.id
	where a.票 = @id and a.还箱时间 is not null
	group by b.承运标志,b.放行时间,c.箱量

)



create function [dbo].[函数更新_业务备案_进口票_通关天数](@id uniqueidentifier)
returns table
as
return
(
	select (DATEDIFF(day,(case when 到港时间 is null or 单证齐全时间 is null then null
								when 到港时间 >= 单证齐全时间 then 到港时间
								when 到港时间 < 单证齐全时间 then 单证齐全时间 end),放行时间)+1) as 通关天数
	from 业务备案_进口票 where id = @id

)


CREATE function [dbo].[函数更新_业务备案_进口票_常规已对账](@id varchar(50))
returns table
as
return
(
	select (case when count(id) = 0 then 'False' else 'True' end) as 常规已对账 
	from 视图查询_业务费用明细 
	where 费用实体 = @id AND 对账单号 IS NOT NULL AND 大类='业务常规'

)

CREATE function [dbo].[函数更新_业务备案_进口票_用箱天数](@id uniqueidentifier)
returns table
as
return
(
	select datediff(day,b.到港时间,case when c.箱量 = count(a.id) then max(a.还箱时间) else null end) as 用箱天数
    from 业务备案_进口箱 as a inner join 业务备案_进口票 as b on a.票 = b.id inner join 业务备案_普通票 as c on c.Id = b.id
	where a.票 = @id and a.还箱时间 is not null
	group by c.箱量,b.到港时间

)


create function [dbo].[函数更新_业务备案_进口票_滞箱费标志](@id varchar(50))
returns table
as
return
(
select top 1 b.滞箱费标志
from 业务备案_进口票 as a left join 业务备案_进口箱 as b on a.id = b.票 
where a.id = @id group by b.滞箱费标志 order by b.滞箱费标志
)




CREATE  function [dbo].[函数更新_业务备案_进口票_最终免箱标志](@id varchar(50))
returns table
as
return
(
select (case when b.箱量 = count(c.最终免箱天数) then 'True' else 'False' end) as 最终免箱标志
from 业务备案_进口票 as a left join 业务备案_普通票 as b on a.id = b.id
left join 业务备案_进口箱 as c on a.id = c.票 where a.id = @id group by b.箱量

)










CREATE function [dbo].[函数更新_业务备案_进口箱](@id varchar(50))
returns table
as
return
(
select dateadd(day,15,b.到港时间) as 货代提箱时间要求止,datediff(day,b.到港时间,a.还箱时间) as 用箱天数,datediff(day,b.放行时间,a.还箱时间) as 承运天数,
(case when 还箱时间 is null then null when datediff(day,b.到港时间,a.还箱时间) <= a.最终免箱天数 or datediff(day,b.到港时间,a.还箱时间) <= b.免箱天数 then 'True' else 'False' end) as 滞箱费标志
from 业务备案_进口箱 as a left join 业务备案_进口票 as b on a.票 = b.id where a.id= @id
)











CREATE function [dbo].[函数查询_业务财务_额外费用过程管理](@对账单 varchar(20))
returns table
as
return
(
	select a.费用实体,a.货代自编号,@对账单 as 对账单号,b.费用项编号,b.费用项,c.submitted,c.完全标志付,c.委托人承担,c.车队承担,c.对外付款,c.自己承担
    from 函数查询_对账单_常规额外_票(@对账单) as a cross join
	(select distinct 费用项编号,费用项 from 报表_对账单 where 对账单 = @对账单) as b
	left join 财务_费用信息 as c on a.费用实体 = c.票 and c.费用项 = b.费用项编号
	group by a.费用实体,a.货代自编号,b.费用项编号,b.费用项,c.submitted,c.完全标志付,c.委托人承担,c.车队承担,c.对外付款,c.自己承担

)





CREATE function [dbo].[函数查询_业务费用小计_票费用项_报表值](@id uniqueidentifier)--,@费用项 int,@收付标志 int)
returns table
as
return
(
--	select 	(case when b.submitted = 1 then sum(a.金额) 
--					when b.submitted = 0 then sum(case when c.submitted = 1 or a.凭证费用明细 is not null then a.金额 
--														--when  
--														end) 
--					end) as 报表值金额
--	from 财务_费用 as a 
--	inner join 财务_费用信息 as b on a.费用实体 = b.票 and a.费用项 = b.费用项 
--	left join 财务_对账单 as c on a.对账单 = c.id 
--	where 费用实体 = @id and a.费用项 = @费用项 and a.收付标志 = @收付标志
--	group by b.submitted
select a.编号 as 费用项, d.Id as 费用实体,
(select sum(金额) as 拟收金额 from 财务_费用 
	where 收付标志 = 1 
	and 费用实体 = d.id and 费用项 = a.编号 ) as 报表值收1,

(select sum(case when 实际值 = 0 then 理论值 else 实际值 end) from 视图查询_报表值_票箱费用项 
where 费用实体 = d.id and 费用项 = a.编号 and 收付标志 = 1)  as 报表值收2,

(select sum(金额) as 拟收金额 from 财务_费用 
	where 收付标志 = 2 
	and 费用实体 = d.id and 费用项 = a.编号 ) as 报表值付1,

(select sum(case when 实际值 = 0 then 理论值 else 实际值 end) from 视图查询_报表值_票箱费用项 
where 费用实体 = d.id and 费用项 = a.编号 and 收付标志 = 2)  as 报表值付2
         
FROM         dbo.参数备案_费用项 AS a INNER JOIN
                      dbo.财务_费用实体 AS d ON d.Id =@id and a.现有费用实体类型 LIKE '%'+convert(varchar(2),d.费用实体类型)+'%' LEFT OUTER JOIN
                      dbo.财务_费用信息 AS b ON a.编号 = b.费用项 AND d.Id = b.票

)








CREATE function [dbo].[函数查询_报表_固定资产明细](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	 select sum(a.月末估值) as 月初值,b.*
	 from 函数查询_报表_固定资产折旧(dateadd(month,-1,@日期Ge),dateadd(month,-1,@日期Le)) as a inner join
	 函数查询_报表_固定资产折旧(@日期Ge,@日期Le) as b on 1=1
	 group by b.分类,b.日期,b.简介,b.购入金额,b.资产归零时间,b.新旧,b.本月折旧,b.累计折旧,b.月末估值
)







CREATE function [dbo].[函数查询_业务费用小计_票费用项_子](@id uniqueidentifier)
returns table
as
return
(
SELECT     a.编号 as 费用项, a.SeqNo, d.Id as 费用实体, a.收入类别 as 费用类别, d.费用实体类型 as 业务类型, b.Submitted, b.完全标志付,
--已收金额
(SELECT     SUM(E.金额) AS Expr1 
FROM         dbo.财务_费用 AS E 
where e.费用实体=d.id and e.费用项=a.编号 and E.收付标志 = 1  AND (E.凭证费用明细 IS NOT NULL)) as 已收金额,
--已付金额
(SELECT     SUM(E.金额) AS Expr1
FROM         dbo.财务_费用 AS E 
where e.费用实体=d.id and e.费用项=a.编号 and E.收付标志 = 2  AND (E.凭证费用明细 IS NOT NULL)) as 已付金额,

--应收金额
(SELECT     SUM(E.金额) AS Expr1 
FROM         dbo.财务_费用 AS E LEFT OUTER JOIN
                      dbo.财务_对账单 AS F ON E.对账单 = F.Id
where e.费用实体=d.id and e.费用项=a.编号 and E.收付标志 = 1  AND (E.凭证费用明细 IS NOT NULL OR f.submitted =1)) as 应收金额,
--应付金额
(SELECT     SUM(E.金额) AS Expr1
FROM         dbo.财务_费用 AS E LEFT OUTER JOIN
                      dbo.财务_对账单 AS F ON E.对账单 = F.Id
where e.费用实体=d.id and e.费用项=a.编号 and E.收付标志 = 2  AND (E.凭证费用明细 IS NOT NULL OR f.submitted =1)) as 应付金额,
--拟收金额
(select sum(金额) as 拟收金额 from 财务_费用 
	where 费用类型 = 1 and 收付标志 = 1 
	and 费用实体 = d.id and 费用项 = a.编号) as 拟收金额,  
--拟付金额
(select sum(金额) as 拟收金额 from 财务_费用 
	where 费用类型 = 1 and 收付标志 = 2
	and 费用实体 = d.id and 费用项 = a.编号) as 拟付金额,
--理论收
(select sum(金额) as 理论收金额 from 财务_业务费用理论值 
	where 收付标志 = 1 and 费用实体 = d.id and 费用项 = a.编号) as 理论收金额,
--理论收
(select sum(金额) as 理论收金额 from 财务_业务费用理论值 
	where 收付标志 = 2 and 费用实体 = d.id and 费用项 = a.编号) as 理论付金额
--报表值收
--case when b.submitted = 1 then 
--(select sum(case when 实际值 is null then 理论值 else 实际值 end) from 视图查询_报表值_票箱费用项 
--where 费用实体 = d.id and 费用项 = a.编号 and 收付标志 = 1) else (select sum(金额) as 拟收金额 from 财务_费用 
--	where 收付标志 = 1 
--	and 费用实体 = d.id and 费用项 = a.编号 )end as 报表值收
----报表值付   
--case when b.完全标志付 = 0 then 
--(select sum(case when 实际值 is null then 理论值 else 实际值 end) from 视图查询_报表值_票箱费用项 
--where 费用实体 = d.id and 费用项 = a.编号 and 收付标志 = 2) else (select sum(金额) as 拟付金额 from 财务_费用 
--	where 收付标志 = 2 
--	and 费用实体 = d.id and 费用项 = a.编号 )end as 报表值付           
FROM         dbo.参数备案_费用项 AS a INNER JOIN
                      dbo.财务_费用实体 AS d ON d.Id =@id and a.现有费用实体类型 LIKE '%'+convert(varchar(2),d.费用实体类型)+'%' LEFT OUTER JOIN
                      dbo.财务_费用信息 AS b ON a.编号 = b.费用项 AND d.Id = b.票

)



CREATE function [dbo].[函数查询_报表_业务应付款月报表_进口报关](@日期Ge datetime='2009-11-1',@日期Le datetime='2009-11-30')
returns table
as
return
(
	select ut.委托人,d.总票数,d.总箱量,e.月初_常规费用,e.月初_额外费用,e.当月增加_常规费用,e.当月增加_额外费用,e.当月减少_常规费用,e.当月减少_额外费用,e.月末_常规费用,e.月末_额外费用,f.月末前待确认业务_常规费用,f.月末前待确认业务_额外费用,e.备注
	  from (select a.委托人
			 from 函数查询_报表_业务应收应付款月报表_进口报关_当月委托情况(@日期Ge,@日期Le) as a
			 union 
			select b.委托人 
			 from 函数查询_报表_业务应付款月报表_进口报关_业务应付款(@日期Ge,@日期Le) as b
			 union
			select c.委托人 
			 from 函数查询_报表_业务应付款月报表_进口报关_月末前待确认业务(@日期Ge,@日期Le) as c) as ut
	left join 函数查询_报表_业务应收应付款月报表_进口报关_当月委托情况(@日期Ge,@日期Le) as d on ut.委托人 = d.委托人
	left join 函数查询_报表_业务应付款月报表_进口报关_业务应付款(@日期Ge,@日期Le) as e on ut.委托人 = e.委托人
	left join 函数查询_报表_业务应付款月报表_进口报关_月末前待确认业务(@日期Ge,@日期Le) as f on ut.委托人 = f.委托人

)




CREATE function [dbo].[函数查询_报表_业务应付款月报表_内贸出港](@日期Ge datetime='2009-11-1',@日期Le datetime='2009-11-30')
returns table
as
return
(
	select ut.委托人,d.总票数,d.总箱量,e.月初_常规费用,e.月初_额外费用,e.当月增加_常规费用,e.当月增加_额外费用,e.当月减少_常规费用,e.当月减少_额外费用,e.月末_常规费用,e.月末_额外费用,f.月末前待确认业务_常规费用,f.月末前待确认业务_额外费用,e.备注
	  from (select a.委托人
			 from 函数查询_报表_业务应收应付款月报表_内贸出港_当月委托情况(@日期Ge,@日期Le) as a
			 union 
			select b.委托人 
			 from 函数查询_报表_业务应付款月报表_内贸出港_业务应付款(@日期Ge,@日期Le) as b
			 union
			select c.委托人 
			 from 函数查询_报表_业务应付款月报表_内贸出港_月末前待确认业务(@日期Ge,@日期Le) as c) as ut
	left join 函数查询_报表_业务应收应付款月报表_内贸出港_当月委托情况(@日期Ge,@日期Le) as d on ut.委托人 = d.委托人
	left join 函数查询_报表_业务应付款月报表_内贸出港_业务应付款(@日期Ge,@日期Le) as e on ut.委托人 = e.委托人
	left join 函数查询_报表_业务应付款月报表_内贸出港_月末前待确认业务(@日期Ge,@日期Le) as f on ut.委托人 = f.委托人

)




CREATE function [dbo].[函数查询_报表_业务应付款月报表_其它](@日期Ge datetime='2009-11-1',@日期Le datetime='2009-11-30')
returns table
as
return
(
	select ut.委托人,d.总票数,d.总箱量,e.月初_常规费用,e.月初_额外费用,e.当月增加_常规费用,e.当月增加_额外费用,e.当月减少_常规费用,e.当月减少_额外费用,e.月末_常规费用,e.月末_额外费用,f.月末前待确认业务_常规费用,f.月末前待确认业务_额外费用,e.备注
	  from (select a.委托人
			 from 函数查询_报表_业务应收应付款月报表_其它_当月委托情况(@日期Ge,@日期Le) as a
			 union 
			select b.委托人 
			 from 函数查询_报表_业务应付款月报表_其它_业务应付款(@日期Ge,@日期Le) as b
			 union
			select c.委托人 
			 from 函数查询_报表_业务应付款月报表_其它_月末前待确认业务(@日期Ge,@日期Le) as c) as ut
	left join 函数查询_报表_业务应收应付款月报表_其它_当月委托情况(@日期Ge,@日期Le) as d on ut.委托人 = d.委托人
	left join 函数查询_报表_业务应付款月报表_其它_业务应付款(@日期Ge,@日期Le) as e on ut.委托人 = e.委托人
	left join 函数查询_报表_业务应付款月报表_其它_月末前待确认业务(@日期Ge,@日期Le) as f on ut.委托人 = f.委托人

)




CREATE function [dbo].[函数查询_报表_业务应收款月报表_进口报关](@日期Ge datetime='2009-11-1',@日期Le datetime='2009-11-30')
returns table
as
return
(
	select ut.委托人,d.总票数,d.总箱量,e.月初_常规费用,e.月初_额外费用,e.当月增加_常规费用,e.当月增加_额外费用,e.当月减少_常规费用,e.当月减少_额外费用,e.月末_常规费用,e.月末_额外费用,f.月末前待确认业务_常规费用,f.月末前待确认业务_额外费用,e.备注
	  from (select a.委托人
			 from 函数查询_报表_业务应收应付款月报表_进口报关_当月委托情况(@日期Ge,@日期Le) as a
			 union 
			select b.委托人 
			 from 函数查询_报表_业务应收款月报表_进口报关_业务应收款(@日期Ge,@日期Le) as b
			 union
			select c.委托人 
			 from 函数查询_报表_业务应收款月报表_进口报关_月末前待确认业务(@日期Ge,@日期Le) as c) as ut
	left join 函数查询_报表_业务应收应付款月报表_进口报关_当月委托情况(@日期Ge,@日期Le) as d on ut.委托人 = d.委托人
	left join 函数查询_报表_业务应收款月报表_进口报关_业务应收款(@日期Ge,@日期Le) as e on ut.委托人 = e.委托人
	left join 函数查询_报表_业务应收款月报表_进口报关_月末前待确认业务(@日期Ge,@日期Le) as f on ut.委托人 = f.委托人

)



create function [dbo].[函数查询_报表_业务应付款月报表_总表](@日期Ge datetime='2009-11-1',@日期Le datetime='2009-11-30')
returns table
as
return
(
	select ut.业务类型,d.总票数,d.总箱量,e.月初_常规费用,e.月初_额外费用,e.当月增加_常规费用,e.当月增加_额外费用,e.当月减少_常规费用,e.当月减少_额外费用,e.月末_常规费用,e.月末_额外费用,f.月末前待确认业务_常规费用,f.月末前待确认业务_额外费用
	  from (select a.业务类型
			 from 函数查询_报表_业务应收应付款月报表_总表_当月委托情况(@日期Ge,@日期Le) as a
			 union 
			select b.业务类型 
			 from 函数查询_报表_业务应付款月报表_总表_业务应付款(@日期Ge,@日期Le) as b
			 union
			select c.业务类型 
			 from 函数查询_报表_业务应付款月报表_总表_月末前待确认业务(@日期Ge,@日期Le) as c) as ut
	left join 函数查询_报表_业务应收应付款月报表_总表_当月委托情况(@日期Ge,@日期Le) as d on ut.业务类型 = d.业务类型
	left join 函数查询_报表_业务应付款月报表_总表_业务应付款(@日期Ge,@日期Le) as e on ut.业务类型 = e.业务类型
	left join 函数查询_报表_业务应付款月报表_总表_月末前待确认业务(@日期Ge,@日期Le) as f on ut.业务类型 = f.业务类型

)



CREATE function [dbo].[函数查询_报表_业务应收款月报表_内贸出港](@日期Ge datetime='2009-11-1',@日期Le datetime='2009-11-30')
returns table
as
return
(
	select ut.委托人,d.总票数,d.总箱量,e.月初_常规费用,e.月初_额外费用,e.当月增加_常规费用,e.当月增加_额外费用,e.当月减少_常规费用,e.当月减少_额外费用,e.月末_常规费用,e.月末_额外费用,f.月末前待确认业务_常规费用,f.月末前待确认业务_额外费用,e.备注
	  from (select a.委托人
			 from 函数查询_报表_业务应收应付款月报表_内贸出港_当月委托情况(@日期Ge,@日期Le) as a
			 union 
			select b.委托人 
			 from 函数查询_报表_业务应收款月报表_内贸出港_业务应收款(@日期Ge,@日期Le) as b
			 union
			select c.委托人 
			 from 函数查询_报表_业务应收款月报表_内贸出港_月末前待确认业务(@日期Ge,@日期Le) as c) as ut
	left join 函数查询_报表_业务应收应付款月报表_内贸出港_当月委托情况(@日期Ge,@日期Le) as d on ut.委托人 = d.委托人
	left join 函数查询_报表_业务应收款月报表_内贸出港_业务应收款(@日期Ge,@日期Le) as e on ut.委托人 = e.委托人
	left join 函数查询_报表_业务应收款月报表_内贸出港_月末前待确认业务(@日期Ge,@日期Le) as f on ut.委托人 = f.委托人

)




CREATE function [dbo].[函数查询_报表_业务应收款月报表_其它](@日期Ge datetime='2009-11-1',@日期Le datetime='2009-11-30')
returns table
as
return
(
	select ut.委托人,d.总票数,d.总箱量,e.月初_常规费用,e.月初_额外费用,e.当月增加_常规费用,e.当月增加_额外费用,e.当月减少_常规费用,e.当月减少_额外费用,e.月末_常规费用,e.月末_额外费用,f.月末前待确认业务_常规费用,f.月末前待确认业务_额外费用,e.备注
	  from (select a.委托人
			 from 函数查询_报表_业务应收应付款月报表_其它_当月委托情况(@日期Ge,@日期Le) as a
			 union 
			select b.委托人 
			 from 函数查询_报表_业务应收款月报表_其它_业务应收款(@日期Ge,@日期Le) as b
			 union
			select c.委托人 
			 from 函数查询_报表_业务应收款月报表_其它_月末前待确认业务(@日期Ge,@日期Le) as c) as ut
	left join 函数查询_报表_业务应收应付款月报表_其它_当月委托情况(@日期Ge,@日期Le) as d on ut.委托人 = d.委托人
	left join 函数查询_报表_业务应收款月报表_其它_业务应收款(@日期Ge,@日期Le) as e on ut.委托人 = e.委托人
	left join 函数查询_报表_业务应收款月报表_其它_月末前待确认业务(@日期Ge,@日期Le) as f on ut.委托人 = f.委托人

)


create function [dbo].[函数查询_报表_业务应收款月报表_总表](@日期Ge datetime='2009-11-1',@日期Le datetime='2009-11-30')
returns table
as
return
(
	select ut.业务类型,d.总票数,d.总箱量,e.月初_常规费用,e.月初_额外费用,e.当月增加_常规费用,e.当月增加_额外费用,e.当月减少_常规费用,e.当月减少_额外费用,e.月末_常规费用,e.月末_额外费用,f.月末前待确认业务_常规费用,f.月末前待确认业务_额外费用
	  from (select a.业务类型
			 from 函数查询_报表_业务应收应付款月报表_总表_当月委托情况(@日期Ge,@日期Le) as a
			 union 
			select b.业务类型 
			 from 函数查询_报表_业务应收款月报表_总表_业务应收款(@日期Ge,@日期Le) as b
			 union
			select c.业务类型 
			 from 函数查询_报表_业务应收款月报表_总表_月末前待确认业务(@日期Ge,@日期Le) as c) as ut
	left join 函数查询_报表_业务应收应付款月报表_总表_当月委托情况(@日期Ge,@日期Le) as d on ut.业务类型 = d.业务类型
	left join 函数查询_报表_业务应收款月报表_总表_业务应收款(@日期Ge,@日期Le) as e on ut.业务类型 = e.业务类型
	left join 函数查询_报表_业务应收款月报表_总表_月末前待确认业务(@日期Ge,@日期Le) as f on ut.业务类型 = f.业务类型

)



CREATE function [dbo].[函数查询_报表_处理坏账损失明细](@日期Ge datetime = '2005-1-1')
returns table
as
return
(
	select sum((case when 入账日期 < @日期Ge then 金额 else 0 end)) as 累计金额,相关人,金额,入账日期 as 日期,凭证号,
		   dbo.Concatenate(distinct 对账单号) as 备注
	from 报表_处理坏账损失明细 
	group by 相关人,金额,入账日期,凭证号

)








CREATE function [dbo].[函数查询_报表_公司投资他人总额明细](@入账日期Ge datetime='2005-1-1',@入账日期Le datetime='2020-12-31')
returns table
as
return
(
	select 相关人,((sum(case when 日期 < @入账日期Ge then (case when 投资金额 is null then 0 else 投资金额 end) end)) - (sum(case when 日期 < @入账日期Ge then (case when 撤资金额 is null then 0 else 撤资金额 end) end))) as 月初值,
	sum(case when 日期 between @入账日期Ge and @入账日期Le then 投资金额 end) as 本月增加,
    sum(case when 日期 between @入账日期Ge and @入账日期Le then 撤资金额 end) as 本月减少,类型 from 报表_非业务费用明细_公司投资他人增减明细 
    group by 相关人,类型
	
)










CREATE function [dbo].[函数查询_报表_固定资产折旧](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	SELECT     b.简介, (case when b.购入时间 < @日期Ge then '原有' else '新购' end) as 新旧, b.分类, b.购入时间 as 日期, b.购入金额, 
			SUM(case when a.入账日期 between @日期Ge and @日期Le then a.付款金额 else 0 end) AS 本月折旧, 
			SUM(case when a.入账日期 <= @日期Le then a.付款金额 else 0 end) AS 累计折旧, 
			(b.购入金额 - (case when a.入账日期 <= @日期Le then a.付款金额 else 0 end)) AS 月末估值, 
			ISNULL(b.卖出时间, DATEADD(YY, b.使用年限, b.购入时间)) AS 资产归零时间
	FROM         dbo.视图查询_非业务费用明细 AS a INNER JOIN
				 dbo.财务_固定资产 AS b ON a.费用实体 = b.Id
	WHERE     (a.小类 = '资产折旧') AND (a.收付标志 = 2)
	GROUP BY b.简介, b.分类, b.购入时间, b.购入金额, b.卖出时间, b.使用年限, a.入账日期, a.付款金额
)







create function 函数查询_业务财务_额外费用过程管理_应付额外(@对账单 varchar(20))
returns table
as
return
(
	select 费用实体,b.编号 as 货代自编号,a.编号 as 对账单号,费用项 as 费用项编号,a.submitted,完全标志付,委托人承担,车队承担,对外付款,自己承担
	from 视图查询_对账单_票费用项金额 as a inner join 财务_费用实体 as b on a.费用实体 = b.id
	where 对账单类型 = 3 and 常规额外 = 001 and a.编号 = @对账单
)

CREATE function [dbo].[函数查询_报表_公司投资他人总体情况](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select sum(月初值) as 月初值,sum(本月增加) as 本月增加,sum(本月减少) as 本月减少,sum(isnull(月初值,0)+isnull(本月增加,0)-isnull(本月减少,0)) as 月末值
	from 函数查询_报表_公司投资他人总额明细(@日期Ge,@日期Le)

)


CREATE function [dbo].[函数查询_报表_业务盈亏预估表](@日期Ge datetime = '2005-1-1',@日期Le datetime = '2020-12-31')
returns table
as
return
(
	SELECT b.*,c.大类
	FROM  (SELECT id FROM 视图信息_费用实体_业务 where 票时间 between @日期Ge and @日期Le) AS a 
	CROSS apply dbo.函数查询_业务费用小计_票费用项(a.id) as b 
	left join 视图费用类别_业务 AS c ON b.费用类别 = c.代码 AND b.业务类型 = c.业务类型
)

create function 函数查询_报表_进口报关毛利逐票明细(@日期Ge datetime = '2005-1-1',@日期Le datetime = '2020-12-31')
returns table
as
return
(
	SELECT c.货代自编号,c.委托人,c.货物类别,c.转关标志,c.委托时间,c.到港时间,c.放行时间,c.箱量,
			sum(case when d.大类 = '业务常规' then 应收金额 else 0 end) as 常规收入_已对账,
			sum(case when d.大类 = '业务常规' and submitted = 1 then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end) as 常规收入_已分离未对账,
			sum(case when d.大类 = '业务常规' and (submitted <> 1 or submitted is null) then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end) as 常规收入_未分离,
			sum(case when d.大类 = '业务额外' then 应收金额 else 0 end) as 额外收入_已对账,
			sum(case when d.大类 = '业务额外' and submitted = 1 then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end) as 额外收入_已分离未对账,
			sum(case when d.大类 = '业务额外' and (submitted <> 1 or submitted is null) then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end) as 额外收入_未分离,
			
			sum(case when d.大类 = '业务常规' then 应付金额 else 0 end) as 常规支出_已对账,
			sum(case when d.大类 = '业务常规' and 完全标志付 = 1 then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end) as 常规支出_已分离未对账,
			sum(case when d.大类 = '业务常规' and (完全标志付 <> 1 or 完全标志付 is null) then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end) as 常规支出_未分离,
			sum(case when d.大类 = '业务额外' then 应付金额 else 0 end) as 额外支出_已对账,
			sum(case when d.大类 = '业务额外' and 完全标志付 = 1 then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end) as 额外支出_已分离未对账,
			sum(case when d.大类 = '业务额外' and (完全标志付 <> 1 or 完全标志付 is null) then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end) as 额外支出_未分离,
			sum(case when d.大类 = '业务其他' then 报表付金额 else 0 end) as 其他开支预估
	FROM  (SELECT id FROM 业务备案_进口票 where 放行时间 between @日期Ge and @日期Le) AS a 
	CROSS apply dbo.函数查询_业务费用小计_票费用项(a.id) as b 
	inner join 视图查询_票信息_进口 as c  on b.费用实体 = c.id
	left join 视图费用类别_业务 AS d ON b.费用类别 = d.代码 AND b.业务类型 = d.业务类型
	group by c.货代自编号,c.委托人,c.货物类别,c.转关标志,c.委托时间,c.到港时间,c.放行时间,c.箱量

)

CREATE function [dbo].[函数查询_报表_进口报关应收款明细表_常规费用应收款](@日期Ge datetime='2005-1-1' , @日期Le datetime='2020-12-31')
returns table
as
return
(
	SELECT     TOP (100) PERCENT a.委托人, a.货代自编号, a.代表性箱号, a.货物类别, a.转关标志, a.箱量, a.委托时间, a.到港时间, a.放行时间, b.金额, b.确认时间, 
                      b.对账单号, b.到期时间
	FROM         dbo.视图查询_票信息_进口 AS a INNER JOIN
						  dbo.函数查询_报表_进口报关应收款明细表_常规费用应收款_金额('2009-11-1', '2009-11-30') AS b ON a.Id = b.费用实体
	ORDER BY a.委托人, a.委托时间 
)


create function 函数查询_报表_进口报关盈亏简明表(@日期Ge datetime = '2005-1-1',@日期Le datetime = '2020-12-31')
returns table
as
return
(
	SELECT c.委托人,sum(c.箱量) as 箱量,
			sum(case when d.大类 = '业务常规' then 应收金额 else 0 end) as 常规收入_已对账,
			sum(case when d.大类 = '业务常规' and submitted = 1 then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end) as 常规收入_已分离未对账,
			sum(case when d.大类 = '业务常规' and (submitted <> 1 or submitted is null) then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end) as 常规收入_未分离,
			sum(case when d.大类 = '业务额外' then 应收金额 else 0 end) as 额外收入_已对账,
			sum(case when d.大类 = '业务额外' and submitted = 1 then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end) as 额外收入_已分离未对账,
			sum(case when d.大类 = '业务额外' and (submitted <> 1 or submitted is null) then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end) as 额外收入_未分离,
			
			sum(case when d.大类 = '业务常规' then 应付金额 else 0 end) as 常规支出_已对账,
			sum(case when d.大类 = '业务常规' and 完全标志付 = 1 then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end) as 常规支出_已分离未对账,
			sum(case when d.大类 = '业务常规' and (完全标志付 <> 1 or 完全标志付 is null) then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end) as 常规支出_未分离,
			sum(case when d.大类 = '业务额外' then 应付金额 else 0 end) as 额外支出_已对账,
			sum(case when d.大类 = '业务额外' and 完全标志付 = 1 then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end) as 额外支出_已分离未对账,
			sum(case when d.大类 = '业务额外' and (完全标志付 <> 1 or 完全标志付 is null) then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end) as 额外支出_未分离,
			sum(case when d.大类 = '业务其他' then 报表付金额 else 0 end) as 其他开支预估
	FROM  (SELECT id FROM 业务备案_进口票 where 放行时间 between @日期Ge and @日期Le) AS a 
	CROSS apply dbo.函数查询_业务费用小计_票费用项(a.id) as b 
	inner join 视图查询_票信息_进口 as c  on b.费用实体 = c.id
	left join 视图费用类别_业务 AS d ON b.费用类别 = d.代码 AND b.业务类型 = d.业务类型
	group by c.委托人
)
create function [dbo].[函数查询_报表_进口报关应收款明细表_待确认业务_常规费用应收款_已收金额](@日期Le datetime='2020-12-31')
returns table
as
return
(
	--月末前所产生的待确认的可能成为业务应收款预估明细
	select 相关人,(sum(已分离金额) + sum(未分离金额)) as 已收金额 from 视图查询_业务费用小计
	where 票时间 <= @日期Le and 收付标志 = 1 and 大类 = '业务常规'
	group by 相关人
)



CREATE function [dbo].[函数查询_报表_他人投资公司总体情况](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select sum(月初值) as 月初值,sum(本月增加) as 本月增加,sum(本月减少) as 本月减少,sum(isnull(月初值,0)+isnull(本月增加,0)-isnull(本月减少,0)) as 月末值
	from 函数查询_报表_他人投资公司总额明细(@日期Ge,@日期Le)

)






CREATE function [dbo].[函数查询_报表_他人投资公司总额明细](@入账日期Ge datetime='2005-1-1',@入账日期Le datetime='2020-12-31')
returns table
as
return
(
	select 相关人,((sum(case when 日期 < @入账日期Ge then (case when 投资金额 is null then 0 else 投资金额 end) end)) - (sum(case when 日期 < @入账日期Ge then (case when 撤资金额 is null then 0 else 撤资金额 end) end))) as 月初值,
	sum(case when 日期 between @入账日期Ge and @入账日期Le then 投资金额 end) as 本月增加,
    sum(case when 日期 between @入账日期Ge and @入账日期Le then 撤资金额 end) as 本月减少,类型 from 报表_非业务费用明细_他人投资公司增减明细 
    group by 相关人,类型
	
)








create function [dbo].[函数更新_业务备案_进口票_预计用箱天数](@id uniqueidentifier)
returns table
as
return
(
	select datediff(day,b.到港时间,case when c.箱量 = count(a.id) then max(a.提箱时间) else null end)+2 as 预计用箱天数
    from 业务备案_进口箱 as a inner join 业务备案_进口票 as b on a.票 = b.id inner join 业务备案_普通票 as c on c.Id = b.id
	where a.票 = @id and a.提箱时间 is not null
	group by c.箱量,b.到港时间
)


CREATE function [dbo].[函数查询_报表_业务应付款月报表_进口报关_月末前待确认业务](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select b.委托人,isnull(sum(case when a.大类 = '业务常规' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务常规' then a.未分离金额 else 0 end),0) as 月末前待确认业务_常规费用,
	       isnull(sum(case when a.大类 = '业务额外' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务额外' then a.未分离金额 else 0 end),0) as 月末前待确认业务_额外费用
	from 报表_业务盈亏预估表 as a inner join 报表_进口报关业务明细 as b on a.费用实体 = b.id
	where a.票时间 between @日期Ge and @日期Le and a.费用实体类型 = 11 and a.收付标志 = 2
	group by b.委托人
)





CREATE function [dbo].[函数查询_报表_业务应付款月报表_其它_月末前待确认业务](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select b.委托人,isnull(sum(case when a.大类 = '业务常规' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务常规' then a.未分离金额 else 0 end),0) as 月末前待确认业务_常规费用,
	       isnull(sum(case when a.大类 = '业务额外' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务额外' then a.未分离金额 else 0 end),0) as 月末前待确认业务_额外费用
	from 报表_业务盈亏预估表 as a inner join 视图查询_票信息_进口其他业务 as b on a.费用实体 = b.id
	where a.票时间 between @日期Ge and @日期Le and a.费用实体类型 = 45 and a.收付标志 = 2
	group by b.委托人
)







CREATE function [dbo].[函数查询_报表_贷款明细](@日期Ge datetime = '2005-1-1',@日期Le datetime = '2020-12-31')
returns table
as
return
(
	select (sum((case when a.日期 < @日期Ge then a.金额 else 0 end))-sum((case when a.结算期限 < @日期Ge then a.金额 else 0 end))) as 月初值,b.* 
	from 报表_贷款明细  as a right join 报表_贷款明细 as b on 1=1
	--where b.日期 between @日期Ge and @日期Le 
	group by b.日期,b.结算期限,b.业务类型,b.费用项,b.金额,b.相关人,b.收付标志,b.凭证号,b.收付款方式,b.用途,b.备注

)










CREATE function [dbo].[函数查询_报表_借出款明细](@日期Ge datetime = '2005-1-1',@日期Le datetime = '2020-12-31')
returns table
as
return
(
	select (sum((case when a.日期 < @日期Ge then a.金额 else 0 end))-sum((case when a.结算期限 < @日期Ge then a.金额 else 0 end))) as 月初值,b.* 
	from 报表_借出款明细  as a right join 报表_借出款明细 as b on 1=1
	--where b.日期 between @日期Ge and @日期Le 
	group by b.日期,b.结算期限,b.业务类型,b.费用项,b.金额,b.相关人,b.收付标志,b.凭证号,b.收付款方式,b.用途,b.备注
	
)








CREATE function [dbo].[函数查询_报表_业务应付款月报表_总表_月末前待确认业务](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table   
as
return
(
	select b.业务类型,isnull(sum(case when a.大类 = '业务常规' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务常规' then a.未分离金额 else 0 end),0) as 月末前待确认业务_常规费用,
	       isnull(sum(case when a.大类 = '业务额外' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务额外' then a.未分离金额 else 0 end),0) as 月末前待确认业务_额外费用
	from 报表_业务盈亏预估表 as a inner join 视图查询_票信息_委托人 as b on a.费用实体 = b.id
	where a.票时间 between @日期Ge and @日期Le and a.收付标志 = 2
	group by b.业务类型
)










CREATE function [dbo].[函数查询_报表_进口报关业务明细](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31') 
returns table
as
return
(
	select * from 报表_进口报关业务明细
	where (委托时间 between @日期Ge and @日期Le) or 
		  --(到港时间 between @日期Ge and @日期Le) or 
	      (报关时间 between @日期Ge and @日期Le) or
		  (放行时间 between @日期Ge and @日期Le) or
		  (开始时间 between @日期Ge and @日期Le) or
		  (结束时间 between @日期Ge and @日期Le) or
	      (报关时间 <= @日期Le and 承运标志 = 0 and (放行时间 is null or 放行时间 > @日期Le)) or 
		  (报关时间 <= @日期Le and 承运标志 = 1 and (结束时间 is null or 结束时间 > @日期Le))

)














CREATE function [dbo].[函数查询_报表_进口报关业务月报表](@TimeGe datetime='2005-1-1',@TimeLe datetime='2020-12-31')
returns table
as
RETURN 
(
	select * from 报表_进口报关业务明细 
	where (委托时间 between @TimeGe and @TimeLe) or 
		  --(到港时间 between @TimeGe and @TimeLe) or 
	      (报关时间 between @TimeGe and @TimeLe) or
		  (放行时间 between @TimeGe and @TimeLe) or
		  (开始时间 between @TimeGe and @TimeLe) or
		  (结束时间 between @TimeGe and @TimeLe) or
	      (报关时间 <= @TimeLe and 承运标志 = 0 and (放行时间 is null or 放行时间 > @TimeLe)) or 
		  (报关时间 <= @TimeLe and 承运标志 = 1 and (结束时间 is null or 结束时间 > @TimeLe))
)









create function [dbo].[函数查询_报表_业务应收款月报表_进口报关_月末前待确认业务](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select b.委托人,isnull(sum(case when a.大类 = '业务常规' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务常规' then a.未分离金额 else 0 end),0) as 月末前待确认业务_常规费用,
	       isnull(sum(case when a.大类 = '业务额外' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务额外' then a.未分离金额 else 0 end),0) as 月末前待确认业务_额外费用
	from 报表_业务盈亏预估表 as a inner join 报表_进口报关业务明细 as b on a.费用实体 = b.id
	where a.票时间 between @日期Ge and @日期Le and a.费用实体类型 = 11 and a.收付标志 = 1
	group by b.委托人
)





CREATE function [dbo].[函数查询_报表_进口报关应收款明细表_本月增加_常规费用应收款_已收金额](@日期Ge datetime='2005-1-1' , @日期Le datetime='2020-12-31')
returns table
as
return
(
	--本月所增加的业务应收款及核销明细
	select 相关人,sum(case when 金额 > 0 then 金额 else 0 end) as 已收金额,max(结算期限) as 收款时间,dbo.Concatenate(distinct (case when 源 = '凭证' then 相关号 end)) as 收款凭证号 
	from 视图查询_应收应付明细
	where 结算期限 between @日期Ge and @日期Le and 收付标志 = 1 and 费用项 = 000
	group by 相关人
)



create function [dbo].[函数查询_报表_进口报关应收款明细表_常规费用应收款_金额](@日期Ge datetime='2005-1-1' , @日期Le datetime='2020-12-31')
returns table
as
return
(
	SELECT     费用实体, SUM(金额) AS 金额, MAX(关账日期) AS 确认时间, dbo.Concatenate(DISTINCT 对账单号) AS 对账单号, MAX(结算期限) AS 到期时间
	FROM         dbo.视图查询_对账单_票
	WHERE     (收付标志 = 1) AND (费用类型 = 000) and 关账日期 between @日期Ge and @日期Le
	GROUP BY 费用实体
)




CREATE function [dbo].[函数查询_报表_业务应收款月报表_内贸出港_月末前待确认业务](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select b.委托人,isnull(sum(case when a.大类 = '业务常规' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务常规' then a.未分离金额 else 0 end),0) as 月末前待确认业务_常规费用,
	       isnull(sum(case when a.大类 = '业务额外' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务额外' then a.未分离金额 else 0 end),0) as 月末前待确认业务_额外费用
	from 报表_业务盈亏预估表 as a inner join 视图查询_票信息_内贸出港 as b on a.费用实体 = b.id
	where a.票时间 between @日期Ge and @日期Le and a.费用实体类型 = 15 and a.收付标志 = 1
	group by b.委托人
)







create function [dbo].[函数查询_报表_进口报关应收款明细表_月末_常规费用应收款_已收金额](@日期Le datetime='2020-12-31')
returns table
as
return
(
	--月末所留存的业务应收款及核销明细
	select 相关人,sum(金额) as 已收金额 from 视图查询_应收应付明细
	where 结算期限 <= @日期Le and 收付标志 = 1 and 费用项 = 000
	group by 相关人
)



CREATE function [dbo].[函数查询_报表_修洗箱费_已对账](@id uniqueidentifier, @委托人 varchar(20))
returns table
as
return
(
	select sum(金额) as 已对账 
	from 财务_费用 
	where 箱 = @id and 费用项 = 165 and 相关人 = @委托人 and 对账单 is not null

)





CREATE function [dbo].[函数查询_报表_业务应收款月报表_其它_月末前待确认业务](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table   
as
return
(
	select b.委托人,isnull(sum(case when a.大类 = '业务常规' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务常规' then a.未分离金额 else 0 end),0) as 月末前待确认业务_常规费用,
	       isnull(sum(case when a.大类 = '业务额外' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务额外' then a.未分离金额 else 0 end),0) as 月末前待确认业务_额外费用
	from 报表_业务盈亏预估表 as a inner join 视图查询_票信息_进口其他业务 as b on a.费用实体 = b.id
	where a.票时间 between @日期Ge and @日期Le and a.费用实体类型 = 45 and a.收付标志 = 1
	group by b.委托人
)





CREATE function [dbo].[函数查询_报表_业务应付款月报表_进口报关_业务应付款](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select 相关人 as 委托人,
		   sum(case when 日期 < @日期Ge and 费用项 = '000' then 金额 else 0 end) as 月初_常规费用,
		   sum(case when 日期 < @日期Ge and 费用项 = '001' then 金额 else 0 end) as 月初_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '000' then 金额 else 0 end) as 当月增加_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '001' then 金额 else 0 end) as 当月增加_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '000' then 0-金额 else 0 end) as 当月减少_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '001' then 0-金额 else 0 end) as 当月减少_额外费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '000' then 金额 else 0 end) as 月末_常规费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '001' then 金额 else 0 end) as 月末_额外费用,
	       null as 备注
	from 视图查询_应收应付明细
	where 业务类型 = 11 and 收付标志 = 2
	group by 相关人
)






create function [dbo].[函数查询_报表_业务应付款月报表_内贸出港_月末前待确认业务](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select b.委托人,isnull(sum(case when a.大类 = '业务常规' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务常规' then a.未分离金额 else 0 end),0) as 月末前待确认业务_常规费用,
	       isnull(sum(case when a.大类 = '业务额外' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务额外' then a.未分离金额 else 0 end),0) as 月末前待确认业务_额外费用
	from 报表_业务盈亏预估表 as a inner join 视图查询_票信息_内贸出港 as b on a.费用实体 = b.id
	where a.票时间 between @日期Ge and @日期Le and a.费用实体类型 = 15 and a.收付标志 = 2
	group by b.委托人
)




CREATE function [dbo].[函数查询_报表_业务应付款月报表_内贸出港_业务应付款](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select 相关人 as 委托人,
		   sum(case when 日期 < @日期Ge and 费用项 = '000' then 金额 else 0 end) as 月初_常规费用,
		   sum(case when 日期 < @日期Ge and 费用项 = '001' then 金额 else 0 end) as 月初_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '000' then 金额 else 0 end) as 当月增加_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '001' then 金额 else 0 end) as 当月增加_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '000' then 0-金额 else 0 end) as 当月减少_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '001' then 0-金额 else 0 end) as 当月减少_额外费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '000' then 金额 else 0 end) as 月末_常规费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '001' then 金额 else 0 end) as 月末_额外费用,
		   null as 备注
	from 视图查询_应收应付明细
	where 业务类型 = 15 and 收付标志 = 2
	group by 相关人
)







CREATE function [dbo].[函数查询_报表_业务应收款月报表_总表_月末前待确认业务](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table   
as
return
(
	select b.业务类型,isnull(sum(case when a.大类 = '业务常规' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务常规' then a.未分离金额 else 0 end),0) as 月末前待确认业务_常规费用,
	       isnull(sum(case when a.大类 = '业务额外' then a.已分离金额 else 0 end),0) + isnull(sum(case when a.大类 = '业务额外' then a.未分离金额 else 0 end),0) as 月末前待确认业务_额外费用
	from 报表_业务盈亏预估表 as a inner join 视图查询_票信息_委托人 as b on a.费用实体 = b.id
	where a.票时间 between @日期Ge and @日期Le and a.收付标志 = 1
	group by b.业务类型
)





CREATE function [dbo].[函数查询_报表_业务应付款月报表_其它_业务应付款](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select 相关人 as 委托人,
		   sum(case when 日期 < @日期Ge and 费用项 = '000' then 金额 else 0 end) as 月初_常规费用,
		   sum(case when 日期 < @日期Ge and 费用项 = '001' then 金额 else 0 end) as 月初_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '000' then 金额 else 0 end) as 当月增加_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '001' then 金额 else 0 end) as 当月增加_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '000' then 0-金额 else 0 end) as 当月减少_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '001' then 0-金额 else 0 end) as 当月减少_额外费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '000' then 金额 else 0 end) as 月末_常规费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '001' then 金额 else 0 end) as 月末_额外费用,
		   null as 备注
	from 视图查询_应收应付明细
	where 业务类型 = 45 and 收付标志 = 2
	group by 相关人
)










CREATE function [dbo].[函数查询_报表_业务应收应付款月报表_进口报关_当月委托情况](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select 委托人,count(委托人) as 总票数,sum(箱量) as 总箱量 from 视图查询_票信息_进口
	where 委托时间 between @日期Ge and @日期Le
	group by 委托人

)











CREATE function [dbo].[函数查询_报表_业务应付款月报表_总表_业务应付款](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select 业务类型,
		   sum(case when 日期 < @日期Ge and 费用项 = '000' then 金额 else 0 end) as 月初_常规费用,
		   sum(case when 日期 < @日期Ge and 费用项 = '001' then 金额 else 0 end) as 月初_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '000' then 金额 else 0 end) as 当月增加_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '001' then 金额 else 0 end) as 当月增加_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '000' then 0-金额 else 0 end) as 当月减少_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '001' then 0-金额 else 0 end) as 当月减少_额外费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '000' then 金额 else 0 end) as 月末_常规费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '001' then 金额 else 0 end) as 月末_额外费用,
	       null as 备注
	from 视图查询_应收应付明细 
	where 收付标志 = 2 
	group by 业务类型
)









CREATE function [dbo].[函数查询_报表_业务应收应付款月报表_内贸出港_当月委托情况](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select 委托人,count(委托人) as 总票数,sum(箱量) as 总箱量 from 视图查询_票信息_委托人
	where 委托时间 between @日期Ge and @日期Le 
	group by 委托人

)








CREATE function [dbo].[函数查询_报表_业务应收款月报表_进口报关_业务应收款](@日期Ge datetime='2009-11-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select 相关人 as 委托人,
		   sum(case when 日期 < @日期Ge and 费用项 = '000' then 金额 else 0 end) as 月初_常规费用,
		   sum(case when 日期 < @日期Ge and 费用项 = '001' then 金额 else 0 end) as 月初_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '000' then 金额 else 0 end) as 当月增加_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '001' then 金额 else 0 end) as 当月增加_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '000' then 金额 else 0 end) as 当月减少_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '001' then 金额 else 0 end) as 当月减少_额外费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '000' then 金额 else 0 end) as 月末_常规费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '001' then 金额 else 0 end) as 月末_额外费用,
	       null as 备注
	from 视图查询_应收应付明细
	where 业务类型 = 11 and 收付标志 = 1
	group by 相关人

)











CREATE function [dbo].[函数查询_报表_业务应收应付款月报表_其它_当月委托情况](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select 委托人,count(委托人) as 总票数,sum(箱量) as 总箱量 from 视图查询_票信息_进口其他业务
	where 委托时间 between @日期Ge and @日期Le 
	group by 委托人
)












CREATE function [dbo].[函数查询_报表_业务应收款月报表_内贸出港_业务应收款](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select 相关人 as 委托人,
		   sum(case when 日期 < @日期Ge and 费用项 = '000' then 金额 else 0 end) as 月初_常规费用,
		   sum(case when 日期 < @日期Ge and 费用项 = '001' then 金额 else 0 end) as 月初_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '000' then 金额 else 0 end) as 当月增加_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '001' then 金额 else 0 end) as 当月增加_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '000' then 0-金额 else 0 end) as 当月减少_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '001' then 0-金额 else 0 end) as 当月减少_额外费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '000' then 金额 else 0 end) as 月末_常规费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '001' then 金额 else 0 end) as 月末_额外费用,
		   null as 备注
	from 视图查询_应收应付明细
	where 业务类型 = 15 and 收付标志 = 1
	group by 相关人

)










CREATE function [dbo].[函数查询_报表_业务应收应付款月报表_总表_当月委托情况](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select 业务类型,count(id) as 总票数,sum(箱量) as 总箱量 from 视图查询_票信息_委托人
	where 委托时间 between @日期Ge and @日期Le
	group by 业务类型

)




CREATE function [dbo].[函数查询_报表_业务应收款月报表_其它_业务应收款](@日期Ge datetime='2009-11-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select 相关人 as 委托人,
		   sum(case when 日期 < @日期Ge and 费用项 = '000' then 金额 else 0 end) as 月初_常规费用,
		   sum(case when 日期 < @日期Ge and 费用项 = '001' then 金额 else 0 end) as 月初_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '000' then 金额 else 0 end) as 当月增加_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '001' then 金额 else 0 end) as 当月增加_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '000' then 金额 else 0 end) as 当月减少_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '001' then 金额 else 0 end) as 当月减少_额外费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '000' then 金额 else 0 end) as 月末_常规费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '001' then 金额 else 0 end) as 月末_额外费用,
		   null as 备注
	from 视图查询_应收应付明细
	where 业务类型 = 45 and 收付标志 = 1
	group by 相关人

)









CREATE function [dbo].[函数查询_报表_业务应收款月报表_总表_业务应收款](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select 业务类型,
		   sum(case when 日期 < @日期Ge and 费用项 = '000' then 金额 else 0 end) as 月初_常规费用,
		   sum(case when 日期 < @日期Ge and 费用项 = '001' then 金额 else 0 end) as 月初_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '000' then 金额 else 0 end) as 当月增加_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 and 费用项 = '001' then 金额 else 0 end) as 当月增加_额外费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '000' then 0-金额 else 0 end) as 当月减少_常规费用,
		   sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 and 费用项 = '001' then 0-金额 else 0 end) as 当月减少_额外费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '000' then 金额 else 0 end) as 月末_常规费用,
		   sum(case when 日期 < dateadd(month,1,@日期Ge) and 费用项 = '001' then 金额 else 0 end) as 月末_额外费用,
	       null as 备注
	from 视图查询_应收应付明细 
	where 收付标志 = 1 
	group by 业务类型


)









CREATE function [dbo].[函数查询_报表_滞箱费_已对账](@id uniqueidentifier, @委托人 varchar(20))
returns table
as
return
(
	SELECT  sum(金额) AS 已对账
     FROM    dbo.财务_费用
     WHERE  (费用实体 = @id) AND (费用项 = 167) AND (相关人 = @委托人) AND 对账单 is not null
    
)











CREATE function [dbo].[函数查询_报表_资金流水_总体情况](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select (sum(case when 日期 < @日期Ge and 收付款方式 <> 4 and 币制 = 'CNY' then isnull(收入金额,0) else 0 end) - sum(case when 日期 < @日期Ge and 收付款方式 <> 4 and 币制 = 'CNY' then isnull(支出金额,0) else 0 end)) 
	as 月初值_CNY,
	(sum(case when 日期 < @日期Ge and 收付款方式 <> 4 and 币制 = 'USD' then isnull(收入金额,0) else 0 end) - sum(case when 日期 < @日期Ge and 收付款方式 <> 4 and 币制 = 'USD' then isnull(支出金额,0) else 0 end)) 
	as 月初值_USD,
	(sum(case when 日期 < @日期Ge and 收付款方式 = 4 then isnull(收入金额,0) else 0 end) - sum(case when 日期 < @日期Ge and 收付款方式 = 4 then isnull(支出金额,0) else 0 end))
    as 承兑汇票月初值,
	(sum(case when 日期 < @日期Ge and 收付款方式 <> 4 and 币制 = 'CNY' then isnull(收入金额,0) else 0 end)) 
	as 现金存款本月增加,
	(sum(case when 日期 < @日期Ge and 收付款方式 <> 4 and 币制 = 'CNY' then isnull(支出金额,0) else 0 end))  
	as 现金存款本月减少,
	(sum(case when 日期 < @日期Ge and 收付款方式 = 4 then isnull(收入金额,0) else 0 end)) 
	as 承兑汇票本月增加,
	(sum(case when 日期 < @日期Ge and 收付款方式 = 4 then isnull(支出金额,0) else 0 end)) 
	as 承兑汇票本月减少
	from 报表_资金流水 
)





CREATE function [dbo].[函数查询_报表_资金明细_承兑汇票](@日期Le datetime='2020-12-31')
returns table
as
return
(
	SELECT     所属银行, 来源, 票号, 金额, 托收贴现, 到期时间, 返回时间, 日期
	FROM         dbo.视图查询_承兑汇票_凭证收付
	WHERE     收付标志 = 1 AND 日期 <= @日期Le and 票据号码 NOT IN
                          (SELECT     票据号码
                            FROM          dbo.视图查询_承兑汇票_凭证收付 AS 视图查询_承兑汇票_凭证收付_1
                            WHERE      收付标志 = 2 and 日期 <= @日期Le)
)


create function [dbo].[函数查询_报表_资金明细_手头现金](@日期Le datetime='2009-11-30')
returns table
as
return
(
	select sum(case when 存取标志 = 1 and 币制 = 'CNY' then 金额 else 0 end) - sum(case when 存取标志 = 2 and 币制 = 'CNY' then 金额 else 0 end) 手头现金_CNY,
		   sum(case when 存取标志 = 1 and 币制 = 'USD' then 金额 else 0 end) - sum(case when 存取标志 = 2 and 币制 = 'USD' then 金额 else 0 end) 手头现金_USD
	from 视图查询_现金日记帐 
	where 日期 <= @日期Le

)


CREATE function [dbo].[函数更新_业务备案_内贸出港票_常规已对账](@id varchar(50))
returns table
as
return
(
	select (case when count(id) = 0 then 'False' else 'True' end) as 常规已对账 
	from 视图查询_业务费用明细 
	where 费用实体 = @id AND 对账单号 IS NOT NULL AND 大类='业务常规'

)




CREATE function [dbo].[函数查询_报表_资金明细_银行存款](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
returns table
as
return
(
	select 开户银行,币制,账号,sum((case when 日期 < @日期Ge then 本月增减 else 0 end)) as 月初值,
		   sum((case when 日期 between @日期Ge and @日期Le then 本月增减 else 0 end)) as 本月增减,null as 备注 
	from 报表_资金明细_银行存款 
	--where 日期 between @日期Ge and @日期Le 
	group by 开户银行,币制,账号
)

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION  [dbo].[函数查询_报表_资金明细_银行存款NO]
(
	@TimeGe datetime, 
    @TimeLe datetime
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT      t2.开户行 AS 开户银行, t2.币制, t2.账号,
月初值=(select sum(本月增减) from 报表_资金明细_银行存款 where t2.账号 = 账号 and 日期<=@TimeGe group by 开户银行,账号,币制),
 CASE WHEN t1.存取标志 = 1 THEN t1.金额 ELSE 0 - t1.金额 END AS 本月增减, t1.日期,
t1.摘要 AS 备注
FROM         dbo.视图查询_银行日记帐 AS t1 LEFT OUTER JOIN
                      dbo.参数备案_银行账户 AS t2 ON t1.银行账户 = t2.Id where 日期>=@TimeGe and 日期<=@TimeLe
GROUP BY t1.存取标志, t2.开户行, t2.币制, t2.账号, t1.日期, t1.金额, t1.摘要, t1.相关号码
)










CREATE function [dbo].[函数查询_对账单_常规额外_票费用项](@对账单 varchar(20))
returns table
as
return
(
	select @对账单 as 对账单号,b.费用项编号,b.费用项,c.委托人,c.货代自编号,c.提单号,c.合同号,c.品名 as 货名,c.产地,c.委托时间,c.到港时间,c.放行时间,c.卸箱地,c.箱量,c.总重量,c.船公司,c.船名航次,
			c.单证晚到,c.免箱联系货主时间,c.最终免箱标志,
			(select 关账日期 from 财务_对账单 where 编号 = @对账单) as 关账日期,'001' as 常规额外,c.对上备注 as 备注
    from 函数查询_对账单_常规额外_票(@对账单) as a cross join
	(select distinct 费用项编号,费用项 from 报表_对账单 where 对账单 = @对账单) as b
	left join 视图查询_票信息_进口 as c on a.货代自编号 = c.货代自编号
	group by b.费用项编号,b.费用项,c.委托人,c.货代自编号,c.提单号,c.合同号,c.品名,c.产地,c.委托时间,c.到港时间,c.放行时间,c.卸箱地,c.箱量,c.总重量,c.船公司,c.船名航次,
			c.单证晚到,c.免箱联系货主时间,c.最终免箱标志,c.对上备注,c.id
)










CREATE function [dbo].[函数查询_对账单_常规额外_票](@对账单号 varchar(20))
returns table
as
return
(
--	select distinct 货代自编号
--    from 报表_对账单 where 对账单 in
--	(select distinct 对账单 from 报表_对账单 where 常规额外 = 000 and 对账单类型 = 2 and 货代自编号 in 
--	(select distinct 货代自编号 from 报表_对账单 where 对账单 = @对账单号))
--	union
--	select distinct 货代自编号 from 报表_对账单 where 对账单 = @对账单号

	select distinct 货代自编号,费用实体
    from 视图信息_对账单_票 where 对账单号 in
	(select distinct 对账单号 from 视图信息_对账单_票 where 费用项 = 000 and 对账单类型 = 2 and 货代自编号 in 
	(select distinct 货代自编号 from 视图信息_对账单_票 where 对账单号 = @对账单号))
	union
	select distinct 货代自编号,费用实体 from 视图信息_对账单_票 where 对账单号 = @对账单号

)
	






CREATE function [dbo].[函数查询_业务费用小计_票费用项](@Id uniqueidentifier)
returns table
as
return
(
	select a.*,(case when a.submitted = 1 then 报表值收1 else 报表值收2 end) as 报表收金额,(case when a.完全标志付 = 1 then 报表值付1 else 报表值付2 end) as 报表付金额 
	from 函数查询_业务费用小计_票费用项_子(@Id) as a left join
	 函数查询_业务费用小计_票费用项_报表值(@Id) as b on a.费用项 = b.费用项
	
)



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[函数查询_固定资产折旧]
(	
	-- Add the parameters for the function here
	@TimeGe datetime , 
	@TimeLe datetime 
)
RETURNS TABLE 
AS
RETURN 
(


	SELECT 分类,购入时间,简介,购入金额,资产归零时间,case when 购入时间< @TimeGe then '原有' else '新购' end AS 新旧, 
round ((DATEDIFF(day,case when @TimeGe <购入时间 then 购入时间 else @TimeGe end,case when @TimeLe <资产归零时间 then @TimeLe else 资产归零时间 end)+3)/30,0)* 月折旧额 as 本月折旧, 
case when @TimeLe <资产归零时间 then DATEDIFF(month,购入时间,@TimeLe)* 月折旧额 else 购入金额 end as 累计折旧, 
case when @TimeLe <资产归零时间 then 购入金额-(DATEDIFF(month,购入时间,@TimeLe)* 月折旧额)else 0 end as 月末估值
FROM 视图查询_固定资产折旧 A 
)






-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[函数查询_凭证]
(	
	-- Add the parameters for the function here
	@凭证号Like nvarchar(50)
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT * from 报表_凭证 where 凭证号 LIKE @凭证号Like
)





CREATE function [dbo].[函数查询_网页_票信息_进口]()
returns table
as
return(
select b.委托时间,b.委托人,a.承运人,a.转关标志 as 通关类别,(case when a.承运标志 = '0' then 'False' else 'True' end) as 委托分类,b.提单号,b.船名航次,a.到港时间,a.卸箱地 as 停靠码头,
		b.箱量,b.标箱量,a.报检时间,a.商检查验时间,a.商检出证时间,
		海关查验时间 = (case when a.转关标志 = 1 then d.指运地海关查验时间 when a.转关标志 = 2 then c.海关查验时间 end),
		a.放行时间,a.结束时间,b.货代自编号,b.报关单号
 from 业务备案_进口票 as a left join 业务备案_普通票 as b on a.id = b.id 
							left join 业务过程_进口票_清关 as c on a.转关标志 = 2 and a.id = c.id
							left join 业务过程_进口票_转关 as d on a.转关标志 = 1 and a.id = d.id
)



CREATE function [dbo].[函数查询_网页_箱信息_进口]()
returns table
as
return(
select 货代自编号 = (select 货代自编号 from 业务备案_普通票 where id = a.票),b.箱号,b.封志号,b.箱型,b.重量,a.提箱地,a.卸货地,
	   (case when a.还箱时间 is not null then '已结束'
		     when a.提箱时间 is null and a.卸货时间 is null and a.还箱时间 is null then '未开始' else '承运中' end) as 当前状态,a.提箱时间,a.卸货时间,a.还箱时间,b.内部备注 as 备注 from 业务备案_进口箱 as a left join 业务备案_普通箱 as b on a.id = b.id
)

create function [dbo].[函数查询_业务费用小计_票费用项_理论收](@id uniqueidentifier,@费用项 int,@收付标志 int)
returns table
as
return
(
	--理论收
	--财务_业务费用理论值
	select sum(金额) as 理论收金额 from 财务_业务费用理论值 
	where 收付标志 = @收付标志 
	and 费用实体 = @id and 费用项 = @费用项 
)


create function [dbo].[函数查询_业务费用小计_票费用项_拟收](@id uniqueidentifier,@费用项 int,@收付标志 int)
returns table
as
return
(
	--拟收
	--财务_费用.费用类型 = 1
	select sum(金额) as 拟收金额 from 财务_费用 
	where 费用类型 = 1 and 收付标志 = @收付标志 
	and 费用实体 = @id and 费用项 = @费用项
)


create proc [dbo].[过程更新_参数_装货地]
as
begin
DELETE 参数_装货地
INSERT INTO 参数_装货地
SELECT A.装货地 AS 名称, B.业务类型,'true' as IsActive  FROM 
业务备案_内贸出港箱 AS A inner join 视图信息_箱票 AS B ON A.Id=B.Id 
WHERE  A.装货地 IS NOT NULL
GROUP BY A.装货地,B.业务类型
end


CREATE proc [dbo].[过程更新_业务备案_内贸出港票]
as
begin


update 业务备案_内贸出港票 set 更新时间 = getdate(),

		常规已对账 = (select 常规已对账 from 函数更新_业务备案_内贸出港票_常规已对账(a.id))
from 业务备案_内贸出港票 as a where 开航日期 is null or datediff(day,开航日期,getdate()) between 0 and 90


end
















CREATE proc [dbo].[过程更新_业务备案_普通票]
as
begin
	update 业务备案_普通票 set 品名 = (SELECT dbo.Concatenate(DISTINCT X.品名) AS Expr1 FROM dbo.业务备案_普通箱 AS X 
																	INNER JOIN dbo.视图信息_箱票 AS Y ON Y.Id = X.Id
																	WHERE (Y.票 = a.id)),
							   标箱量 = (select sum(case when c.箱型 >= 40 then 2 else 1 end) 
											from 视图信息_箱票 as b inner join 业务备案_普通箱 as c on b.id = c.id
											where b.票 = a.id)
	from 业务备案_普通票 as a

end








CREATE proc [dbo].[过程添加字段_业务备案_进口票]
as
begin
--添加字段(单证晚到，通关天数，结束时间，承运超期，更新时间，用箱天数，最终免箱标志，滞箱费标志，常规已对账)
if not exists(select * from information_schema.columns where table_name = '业务备案_进口票' and column_name = '单证晚到')
begin alter table 业务备案_进口票 add 单证晚到 int null end
if not exists(select * from information_schema.columns where table_name = '业务备案_进口票' and column_name = '通关天数')
begin alter table 业务备案_进口票 add 通关天数 int null end
if not exists(select * from information_schema.columns where table_name = '业务备案_进口票' and column_name = '结束时间')
begin alter table 业务备案_进口票 add 结束时间 datetime null end
if not exists(select * from information_schema.columns where table_name = '业务备案_进口票' and column_name = '承运超期')
begin alter table 业务备案_进口票 add 承运超期 int null end
if not exists(select * from information_schema.columns where table_name = '业务备案_进口票' and column_name = '更新时间')
begin alter table 业务备案_进口票 add 更新时间 datetime null end
if not exists(select * from information_schema.columns where table_name = '业务备案_进口票' and column_name = '用箱天数')
begin alter table 业务备案_进口票 add 用箱天数 int null end
if not exists(select * from information_schema.columns where table_name = '业务备案_进口票' and column_name = '最终免箱标志')
begin alter table 财务_费用信息 add 最终免箱标志 bit null end
if not exists(select * from information_schema.columns where table_name = '业务备案_进口票' and column_name = '滞箱费标志')
begin alter table 财务_费用信息 add 滞箱费标志 bit null end
if not exists(select * from information_schema.columns where table_name = '业务备案_进口票' and column_name = '常规已对账')
begin alter table 财务_费用信息 add 常规已对账 bit null end

--if not exists(select * from information_schema.columns where table_name = '业务备案_普通票' and column_name = '品名')
--begin alter table 业务备案_普通票 add 品名 nvarchar(4000) null end

--if not exists(select * from information_schema.columns where table_name = '业务备案_进口箱' and column_name = '用箱天数')
--begin alter table 业务备案_进口箱 add 用箱天数 int null end
--if not exists(select * from information_schema.columns where table_name = '业务备案_进口箱' and column_name = '承运天数')
--begin alter table 业务备案_进口箱 add 承运天数 int null end

--if not exists(select * from information_schema.columns where table_name = '财务_费用信息' and column_name = '付款创建时间')
--begin alter table 财务_费用信息 add 付款创建时间 datetime null end
--if not exists(select * from information_schema.columns where table_name = '财务_费用信息' and column_name = '付款修改时间')
--begin alter table 财务_费用信息 add 付款修改时间 datetime null end
--if not exists(select * from information_schema.columns where table_name = '财务_费用信息' and column_name = '付款对账凭证状态')
--begin alter table 财务_费用信息 add 付款对账凭证状态 bit null end
--if not exists(select * from information_schema.columns where table_name = '财务_费用信息' and column_name = '收款创建时间')
--begin alter table 财务_费用信息 add 收款创建时间 datetime null end
--if not exists(select * from information_schema.columns where table_name = '财务_费用信息' and column_name = '收款修改时间')
--begin alter table 财务_费用信息 add 收款修改时间 datetime null end
--if not exists(select * from information_schema.columns where table_name = '财务_费用信息' and column_name = '收款对账凭证状态')
--begin alter table 财务_费用信息 add 收款对账凭证状态 bit null end
--if not exists(select * from information_schema.columns where table_name = '财务_费用信息' and column_name = '凭证号')
--begin alter table 财务_费用信息 add 凭证号 nvarchar(8) null end
--if not exists(select * from information_schema.columns where table_name = '财务_费用信息' and column_name = '对账单号')
--begin alter table 财务_费用信息 add 对账单号 nvarchar(8) null end
--if not exists(select * from information_schema.columns where table_name = '财务_费用信息' and column_name = '创建天数')
--begin alter table 财务_费用信息 add 创建天数 int null end
--if not exists(select * from information_schema.columns where table_name = '财务_费用信息' and column_name = '警示标志')
--begin alter table 财务_费用信息 add 警示标志 bit null end
--if not exists(select * from information_schema.columns where table_name = '财务_费用信息' and column_name = '填全状态')
--begin alter table 财务_费用信息 add 填全状态 bit null end
--if not exists(select * from information_schema.columns where table_name = '财务_费用信息' and column_name = '委托人承担')
--begin alter table 财务_费用信息 add 委托人承担 decimal(18,2) null end
--if not exists(select * from information_schema.columns where table_name = '财务_费用信息' and column_name = '车队承担')
--begin alter table 财务_费用信息 add 车队承担 decimal(18,2) null end
--if not exists(select * from information_schema.columns where table_name = '财务_费用信息' and column_name = '对外付款')
--begin alter table 财务_费用信息 add 对外付款 decimal(18,2) null end
--if not exists(select * from information_schema.columns where table_name = '财务_费用信息' and column_name = '自己承担')
--begin alter table 财务_费用信息 add 自己承担 decimal(18,2) null end

end



















CREATE proc [dbo].[过程更新_业务备案_进口票]
as
begin
--update 业务备案_进口票 set 单证晚到 = (select 单证晚到 from 函数更新_业务备案_进口票(a.id)),
--							通关天数 = (select 通关天数 from 函数更新_业务备案_进口票(a.id)),
--							结束时间 = (select 结束时间 from 函数更新_业务备案_进口票(a.id)),
--							承运超期 = (select 承运超期 from 函数更新_业务备案_进口票(a.id)),
--							更新时间 = (select 更新时间 from 函数更新_业务备案_进口票(a.id)),
--							用箱天数 = (select 用箱天数 from 函数更新_业务备案_进口票(a.id)),
--							最终免箱标志 = (select 最终免箱标志 from 函数更新_业务备案_进口票_最终免箱标志(a.id)),
--							滞箱费标志 = (select 滞箱费标志 from 函数更新_业务备案_进口票_滞箱费标志(a.id)),
--						    常规已对账 = (select 常规已对账 from 函数更新_业务备案_进口票_常规已对账(a.id))  
--from 业务备案_进口票 as a where 放行时间 is null or datediff(day,放行时间,getdate()) between 0 and 30

update 业务备案_进口票 set 结束时间 = (select 结束时间 from 函数更新_业务备案_进口票_结束时间(a.id))							
from 业务备案_进口票 as a where 放行时间 is null or datediff(day,放行时间,getdate()) between 0 and 90

update 业务备案_进口票 set 单证晚到 = (select 单证晚到 from 函数更新_业务备案_进口票_单证晚到(a.id)),
							通关天数 = (select 通关天数 from 函数更新_业务备案_进口票_通关天数(a.id)),
							承运超期 = (select 承运超期 from 函数更新_业务备案_进口票_承运超期(a.id)),
							更新时间 = getdate(),
							用箱天数 = case when a.用箱天数 is null then (select 用箱天数 from 函数更新_业务备案_进口票_用箱天数(a.id)) else a.用箱天数 end,
							最终免箱标志 = (select 最终免箱标志 from 函数更新_业务备案_进口票_最终免箱标志(a.id)),
							滞箱费标志 = (select 滞箱费标志 from 函数更新_业务备案_进口票_滞箱费标志(a.id)),
						    常规已对账 = (select 常规已对账 from 函数更新_业务备案_进口票_常规已对账(a.id)),
						    预计用箱天数 = (select 预计用箱天数 from 函数更新_业务备案_进口票_预计用箱天数(a.id)),
							总金额 = case when a.总金额 is null then (select 单价*总重量/1000 from 业务备案_普通票 where id = a.id) else a.总金额 end
from 业务备案_进口票 as a where 放行时间 is null or datediff(day,放行时间,getdate()) between 0 and 90


end



















CREATE proc [dbo].[过程更新_业务备案_进口箱]
as
begin
update 业务备案_进口箱 set 货代提箱时间要求止 = (select 货代提箱时间要求止 from 函数更新_业务备案_进口箱(a.id)),
							用箱天数 = (select 用箱天数 from 函数更新_业务备案_进口箱(a.id)),
							承运天数 = (select 承运天数 from 函数更新_业务备案_进口箱(a.id)),
							滞箱费标志 = (select 滞箱费标志 from 函数更新_业务备案_进口箱(a.id))
from 业务备案_进口箱 as a 
end





create proc [dbo].[过程更新_查询_资金日记帐_行数]
as
begin
delete 查询_资金日记帐_行数
insert into 查询_资金日记帐_行数 (金额,收付标志,ID,币制,源,日期,相关号码,摘要,当前行数)
(select * from 视图查询_资金日记帐_行数)
end

create proc [dbo].[过程更新_查询_现金日记帐_行数]
as
begin
delete 查询_现金日记帐_行数
insert into 查询_现金日记帐_行数 (金额,存取标志,ID,币制,源,日期,相关号码,摘要,当前行数)
(select * from 视图查询_现金日记帐_行数)
end

CREATE proc [dbo].[过程更新_查询_业务费用小计_票汇总]
as
begin
delete 查询_业务费用小计_票汇总 where 费用实体  in (select a.费用实体 from 查询_业务费用小计_票汇总 a where a.费用实体 not in (select b.费用实体 from 视图查询_票费用_最大时间 b))

INSERT INTO 查询_业务费用小计_票汇总 (费用实体) (select a.费用实体 from 视图查询_票费用_最大时间 a where a.费用实体 not in (select b.费用实体 from 查询_业务费用小计_票汇总 b))

update  查询_业务费用小计_票汇总 
			set 费用实体 = h.费用实体,
			    常规已确认收款 = h.常规已确认收款,
			    常规已分离收款 = h.常规已分离收款,
			    常规未分离收款 = h.常规未分离收款,
			    额外已确认收款 = h.额外已确认收款,
			    额外已分离收款 = h.额外已分离收款,
			    额外未分离收款 = h.额外未分离收款,
			    
			    常规已确认付款 = h.常规已确认付款,
			    常规已分离付款 = h.常规已分离付款,
			    常规未分离付款 = h.常规未分离付款,
			    额外已确认付款 = h.额外已确认付款,
			    额外已分离付款 = h.额外已分离付款,
			    额外未分离付款 = h.额外未分离付款,
			    其他开支预估 = h.其他开支预估,
			    更新时间 = getdate()
from 查询_业务费用小计_票汇总 as i inner join
(select 费用实体,
	   sum(case when 大类 = '业务常规' then 应收金额 else 0 end) as 常规已确认收款,
	   sum(case when 大类 = '业务常规' and submitted = 1 then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end) as 常规已分离收款,
	   sum(case when 大类 = '业务常规' and (submitted <> 1 or submitted is null) then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end) as 常规未分离收款, 
	   
	   sum(case when 大类 = '业务额外' then 应收金额 else 0 end) as 额外已确认收款,
	   sum(case when 大类 = '业务额外' and submitted = 1 then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end) as 额外已分离收款,
	   sum(case when 大类 = '业务额外' and (submitted <> 1 or submitted is null) then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end) as 额外未分离收款,
	    
	   sum(case when 大类 = '业务常规' then 应付金额 else 0 end) as 常规已确认付款,
	   sum(case when 大类 = '业务常规' and 完全标志付 = 1 then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end) as 常规已分离付款,
	   sum(case when 大类 = '业务常规' and (完全标志付 <> 1 or 完全标志付 is null) then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end) as 常规未分离付款, 
	   
	   sum(case when 大类 = '业务额外' then 应付金额 else 0 end) as 额外已确认付款,
	   sum(case when 大类 = '业务额外' and 完全标志付 = 1 then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end) as 额外已分离付款,
	   sum(case when 大类 = '业务额外' and (完全标志付 <> 1 or 完全标志付 is null) then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end) as 额外未分离付款,
	   sum(case when 大类 = '业务其他' then 报表付金额 else 0 end) as 其他开支预估	   
FROM  (SELECT b.*,c.大类
	FROM  (SELECT a1.费用实体 FROM 查询_业务费用小计_票汇总 a1 right join 视图查询_票费用_最大时间 a2 on a1.费用实体=a2.费用实体 where a1.更新时间<a2.最大时间 or a1.更新时间 is null) AS a 
	CROSS apply dbo.函数查询_业务费用小计_票费用项(a.费用实体) as b 
	left join 视图费用类别_业务 AS c ON b.费用类别 = c.代码 AND b.业务类型 = c.业务类型) as g
group by 费用实体) as h on i.费用实体=h.费用实体
end
CREATE proc [dbo].[过程更新_财务_费用信息附加字段_滞箱费]
as
begin
	delete 财务_费用信息附加字段_滞箱费
	insert into 财务_费用信息附加字段_滞箱费
	select a.id,
		isnull((select sum(case when 费用项 = 167 and (对账单 is not null or 凭证费用明细 is not null) then 委托人承担 else 0 end) from 视图信息_财务费用信息_委托人承担 where 费用信息 = a.id),0) as 委托人已确认,
		isnull((select sum(case when 费用项 = 167 and (对账单 is null and 凭证费用明细 is null) then 委托人承担 else 0 end) from 视图信息_财务费用信息_委托人承担 where 费用信息 = a.id),0) as 委托人未确认,
		isnull((select sum(case when 费用项 = 167 and (对账单 is not null or 凭证费用明细 is not null) then 车队承担 else 0 end) from 视图信息_财务费用信息_车队承担 where 费用信息 = a.id),0) as 车队已确认,
		isnull((select sum(case when 费用项 = 167 and (对账单 is null and 凭证费用明细 is null) then 车队承担 else 0 end) from 视图信息_财务费用信息_车队承担 where 费用信息 = a.id),0) as 车队未确认,
		isnull((select sum(case when 费用项 = 167 and (对账单 is not null or 凭证费用明细 is not null) then 退滞箱费 else 0 end) from 视图信息_财务费用信息_退滞箱费 where 费用信息 = a.id),0) as 退滞箱费已确认,
		isnull((select sum(case when 费用项 = 167 and (对账单 is null and 凭证费用明细 is null) then 退滞箱费 else 0 end) from 视图信息_财务费用信息_退滞箱费 where 费用信息 = a.id),0) as 退滞箱费未确认,
		isnull((select sum(case when 费用项 = 167 and (对账单 is not null or 凭证费用明细 is not null) then 最终滞箱费 else 0 end) from 视图信息_财务费用信息_最终滞箱费 where 费用信息 = a.id),0) as 最终滞箱费,
		isnull((select sum(case when 费用项 = 167 and (对账单 is null and 凭证费用明细 is null) then 最终滞箱费 else 0 end) from 视图信息_财务费用信息_最终滞箱费 where 费用信息 = a.id),0) as 未确认滞箱费
	from 财务_费用信息 as a where a.费用项 = 167
	
end



create proc [dbo].[过程更新_查询_银行日记帐_行数]
as
begin
delete 查询_银行日记帐_行数
insert into 查询_银行日记帐_行数 (金额,存取标志,ID,银行账户,源,日期,相关号码,摘要,当前行数)
(select * from 视图查询_银行日记帐_行数)
end


create proc [dbo].[过程更新_参数_品名]
as
begin
DELETE 参数_品名
INSERT INTO 参数_品名
SELECT A.品名 AS 名称, B.业务类型,'true' as IsActive  FROM 
业务备案_普通箱 AS A inner join 视图信息_箱票 AS B ON A.Id=B.Id 
WHERE  A.品名 IS NOT NULL
GROUP BY A.品名,B.业务类型

end

create proc [dbo].[过程更新_参数_车号]
as
begin
DELETE 参数_车号
INSERT INTO 参数_车号
SELECT A.车号 AS 名称, B.业务类型,'true' as IsActive  FROM 
业务备案_内贸出港箱 AS A inner join 视图信息_箱票 AS B ON A.Id=B.Id 
WHERE  A.车号 IS NOT NULL
GROUP BY A.车号,B.业务类型
end

CREATE proc [dbo].[过程更新_参数_货物类别]
as
begin
DELETE 参数_货物类别
INSERT INTO 参数_货物类别
SELECT A.货物类别 AS 名称, B.费用实体类型 as 业务类型,'true' as IsActive  FROM 
业务备案_普通票 AS A inner join 财务_费用实体 AS B ON A.Id=B.Id
WHERE (B.费用实体类型=11 OR B.费用实体类型=15 OR B.费用实体类型=45) AND A.货物类别 IS NOT NULL
GROUP BY A.货物类别,B.费用实体类型

end

create proc [dbo].[过程更新_参数_发票单位]
as
begin
DELETE 参数_发票单位
INSERT INTO 参数_发票单位
SELECT A.单位 AS 名称, 'true' as IsActive  FROM 
财务_发票 AS A 
WHERE  A.单位 IS NOT NULL
GROUP BY A.单位

end

create proc [dbo].[过程更新_参数_产地]
as
begin
DELETE 参数_产地
INSERT INTO 参数_产地
SELECT A.产地 AS 名称, 'true' as IsActive  FROM 
业务备案_进口票 AS A 
WHERE  A.产地 IS NOT NULL
GROUP BY A.产地

end





CREATE proc [dbo].[过程更新_财务_费用信息_费用承担]
as
begin
--委托人承担
update 财务_费用信息 set 委托人承担 = (select 委托人承担 from 函数更新_财务_费用信息_委托人承担(a.id)),
--车队承担
						 车队承担 = (select 车队承担 from 函数更新_财务_费用信息_车队承担(a.id)),
--对外付款
						 对外付款 = (select 对外付款 from 函数更新_财务_费用信息_对外付款(a.id))
from 财务_费用信息 as a 


update 财务_费用信息 set 
--自己承担
						 自己承担 = (select isnull(对外付款,0) - isnull(委托人承担,0) - isnull(车队承担,0))
from 财务_费用信息 as a 
end














CREATE proc [dbo].[过程更新_财务_费用信息]
as
begin
update 财务_费用信息 set 最大时间 = b.最大时间
from 财务_费用信息 as a inner join 视图查询_费用信息_最大时间 as b on a.id = b.费用信息

--收款时间
update 财务_费用信息 set --收款创建时间 = (select 收款创建时间 from 函数更新_财务_费用信息_收款时间(a.id)),
					     --收款修改时间 = (select 收款修改时间 from 函数更新_财务_费用信息_收款时间(a.id)),
--付款时间	
						 --付款创建时间 = (select 付款创建时间 from 函数更新_财务_费用信息_付款时间(a.id)),
					     --付款修改时间 = (select 付款修改时间 from 函数更新_财务_费用信息_付款时间(a.id)),

--凭证号
						 凭证号 = (select (case when 凭证号 = '' then Null else 凭证号 end) from 函数更新_财务_费用信息_凭证号(a.id)),
--对账单号	
						 对账单号 = (select (case when 对账单号 = '' then Null else 对账单号 end) from 函数更新_财务_费用信息_对账单号(a.id)),			
--创建天数
						 --创建天数 = (datediff(day,(case when a.付款创建时间 > a.收款创建时间 then a.收款创建时间 
						--								  when a.付款创建时间 < a.收款创建时间 then a.付款创建时间 
						--									when a.付款创建时间 is null or a.收款创建时间 is null then null end),getdate())),
--收款对账凭证状态
						 收款对账凭证状态 = (select 收款对账凭证状态 from 函数更新_财务_费用信息_收款对账凭证状态(a.id)),
--付款对账凭证状态
						 付款对账凭证状态 = (select 付款对账凭证状态 from 函数更新_财务_费用信息_付款对账凭证状态(a.id)),
--警示状态

--填全状态				
						 填全状态 = (select (case when 填全状态 = 0 then 'True' else 'False' end) from 函数更新_财务_费用信息_填全状态(a.id))
from 财务_费用信息 as a 
--inner join 视图查询_费用信息_最大时间 as b on a.id = b.费用信息
--where a.更新时间 < b.最大时间
where a.更新时间<a.最大时间


--委托人承担
update 财务_费用信息 set 委托人承担 = (select 委托人承担 from 函数更新_财务_费用信息_委托人承担(a.id)),
--车队承担
						 车队承担 = (select 车队承担 from 函数更新_财务_费用信息_车队承担(a.id)),
--对外付款
						 对外付款 = (select 对外付款 from 函数更新_财务_费用信息_对外付款(a.id))
from 财务_费用信息 as a 
--inner join 视图查询_费用信息_最大时间 as b on a.id = b.费用信息
--where a.更新时间 < b.最大时间
where a.更新时间<a.最大时间

update 财务_费用信息 set 
						 自己承担 = (select isnull(对外付款,0) - isnull(委托人承担,0) - isnull(车队承担,0)),
更新时间 = getdate()
from 财务_费用信息 as a 
--inner join 视图查询_费用信息_最大时间 as b on a.id = b.费用信息
--where a.更新时间 < b.最大时间
where a.更新时间<a.最大时间
end
create proc [dbo].[过程更新_财务_对账单]
as
begin
	update 财务_对账单 set 凭证号= c.凭证号
	from (select a.对账单,a.凭证号 
	FROM 视图查询_业务费用明细 as a
	where a.对账单 is not null and a.凭证费用明细 is not null group by a.对账单,a.凭证号) as c
	inner join 财务_对账单 as b on c.对账单 = b.id

end



CREATE proc [dbo].[过程查询_进口费用结算单](@对账单 varchar(50),@货代自编号 varchar(50))
as
begin
declare @sql varchar(8000)
set @sql = 'select 对账单,货代自编号,委托人,委托时间,到港时间,产地 as 港口名称,货名,箱量,箱型,卸箱地,船名航次,提单号,合同号,总重量'
select @sql = @sql + ' , max(case 费用项编号 when ''' + 费用项编号 + ''' then 金额 else null end) [' + 费用项 + ']'
from (select distinct 费用项编号,费用项 from 报表_对账单 where 对账单 = @对账单 and 货代自编号 = @货代自编号) as a 
set @sql = @sql + ' from 报表_对账单 where 对账单 = ''' + @对账单 + '''and 货代自编号 ='''+ @货代自编号 +'''group by 对账单,货代自编号,委托人,委托时间,到港时间,产地,货名,箱量,箱型,卸箱地,船名航次,提单号,合同号,总重量'
exec(@sql)

end











CREATE proc [dbo].[过程查询_交叉表_对账单_内贸结算明细](@对账单 varchar(50)='')
as
begin 
declare @sql varchar(8000)
set @sql = 'select 对账单,关账日期,委托人,开航日期,货代自编号,箱量,船名航次,提单号,目的港,箱号,箱型,封志号,装货地,最终目的地,起始日期,包干费'
select @sql = @sql + ' , max(case 费用项编号 when ''' + 费用项编号 + ''' then 金额 else null end) [' + 费用项 + ']'
from (select distinct 费用项编号,费用项 from 报表_内贸结算明细 where 对账单 = @对账单) as a 
set @sql = @sql + ' from 报表_内贸结算明细 where 对账单 ='''+ @对账单 +'''group by 对账单,关账日期,委托人,开航日期,货代自编号,起始日期,箱量,船名航次,提单号,目的港,包干费,箱号,箱型,封志号,装货地,最终目的地 order by 货代自编号'
exec(@sql)

end












CREATE proc [dbo].[过程查询_交叉表_对账单_额外](@对账单 varchar(20))
as
begin
	if exists (select * from sysobjects where name = 'Temp_对账单_常规额外' and xtype = 'u')
	begin
		drop table Temp_对账单_常规额外
	end
	select *,cast(null as varchar(20)) as 金额 into Temp_对账单_常规额外 from 函数查询_对账单_常规额外_票费用项(@对账单)
	
--金额
	update Temp_对账单_常规额外 set 金额 = case when b.对账单 is null and b.金额 = 0 and b.submitted = 1 then '0' 
											when b.对账单 is not null and b.对账单 <> @对账单 and b.submitted = 1 then '已确定' 
											else '未确定' end 
	from Temp_对账单_常规额外 as a left join 视图查询_票费用项_金额 as b on b.货代自编号 = a.货代自编号 and b.费用项 = a.费用项编号 and b.收付标志 = 1

	update Temp_对账单_常规额外 set 金额 = b.金额
	from Temp_对账单_常规额外 as a inner join 视图查询_票费用项_金额 as b on b.货代自编号 = a.货代自编号 and b.费用项 = a.费用项编号
	where b.对账单 = @对账单
		

--	update Temp_对账单_常规额外 set 金额 =c.BBB
--	from (select a.货代自编号,a.费用项,(case when sum(case when b.对账单号 = @对账单 then 1 else 0 end) > 0 then convert(varchar(30),sum(case when b.对账单号 = @对账单 then b.金额 else 0 end)) 
--				when (select 对账单 from 视图查询_票费用项_金额 where 货代自编号 = a.货代自编号 and 费用项 = a.费用项编号) <> @对账单 
--					and (select 对账单 from 视图查询_票费用项_金额 where 货代自编号 = a.货代自编号 and 费用项 = a.费用项编号) is not null 
--				then '已确定'
--				when (select 对账单 from 视图查询_票费用项_金额 where 货代自编号 = a.货代自编号 and 费用项 = a.费用项编号) is null 
--					and (select 金额 from 视图查询_票费用项_金额 where 货代自编号 = a.货代自编号 and 费用项 = a.费用项编号) = 0 
--				then '0'
--				  else '未确定' end) AS BBB
--		from Temp_对账单_常规额外 as a left join 视图查询_业务费用明细 b on b.货代自编号 = a.货代自编号 and b.费用项 = a.费用项编号
--	group by a.货代自编号,a.费用项,a.费用项编号) as c
--	inner join Temp_对账单_常规额外 d on d.货代自编号 = c.货代自编号 and d.费用项 = c.费用项

--生成对账单
	declare @sql varchar(8000)
	set @sql = 'select 对账单,委托人,货代自编号,提单号,合同号,货名,箱量,总重量,卸箱地,船公司,委托时间,到港时间,放行时间,关账日期,常规额外 as 费用项'
	select @sql = @sql + ' , max(case 费用项编号 when ''' + 费用项编号 + ''' then 金额 else null end) [' + 费用项 + ']'
	from (select distinct 费用项编号,费用项 from Temp_对账单_常规额外) as a 
	set @sql = @sql + ',备注,单证晚到,免箱联系货主时间,最终免箱标志 from Temp_对账单_常规额外 group by 对账单,货代自编号,提单号,委托人,箱量,总重量,合同号,船公司,委托时间,到港时间,放行时间,卸箱地,货名,单证晚到,免箱联系货主时间,最终免箱标志,关账日期,备注,常规额外 order by 货代自编号'
	exec(@sql)
end



















CREATE proc [dbo].[过程查询_交叉表_对账单](@对账单 varchar(50))
as
begin
declare @sql varchar(8000)
set @sql = 'select 对账单,委托人,货代自编号,提单号,合同号,货名,箱量,总重量,卸箱地,船公司,委托时间,到港时间,放行时间,关账日期,常规额外 as 费用项,起始日期 as 起始时间'
select @sql = @sql + ' , max(case 费用项编号 when ''' + 费用项编号 + ''' then 金额 else null end) [' + 费用项 + ']'
from (select distinct 费用项编号,费用项 from 报表_对账单 where 对账单 = @对账单) as a 
set @sql = @sql + ',备注,单证晚到,免箱联系货主时间,最终免箱标志 from 报表_对账单 where 对账单 ='''+ @对账单 +''' group by 对账单,货代自编号,提单号,委托人,箱量,总重量,合同号,船公司,委托时间,到港时间,放行时间,备注,卸箱地,货名,单证晚到,免箱联系货主时间,最终免箱标志,关账日期,常规额外,起始日期 order by 货代自编号'
exec(@sql)
end













create proc [dbo].[过程查询_财务月报表_测算值](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
as
begin
declare @业务盈亏情况 decimal(18,2),@非业务类收入合计 decimal(18,2),@管理费用开支合计 decimal(18,2),@本月财务费用开支 decimal(18,2),
@贴息费 decimal(18,2),@营业税金及附加合计 decimal(18,2),@开发票税合计 decimal(18,2),@本月其它开支合计 decimal(18,2)
--业务盈亏情况
select @业务盈亏情况 = (sum(case when 收付标志 = 1 then 金额小计 else 0 end) - sum(case when 收付标志 = 2 then 金额小计 else 0 end)) from 报表_业务盈亏预估表 
	where 票时间 between @日期Ge and @日期Le

--非业务类收入合计
select @非业务类收入合计 = sum(case when 入账日期 between @日期Ge and @日期Le and 收付标志 = 1 then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 源 = '非业务'

--管理费用开支合计
select @管理费用开支合计 = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 like '%管理费用%'

--本月财务费用开支
select @本月财务费用开支 = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 like '%财务费用%' 

--贴息费
select @贴息费 = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 = '财务费用' and 小类 = '贴息' 

--营业税金及附加合计
select @营业税金及附加合计 = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 = '营业税金及附加'

--开发票税合计
select @开发票税合计 = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 = '营业税金及附加' and 小类 = '开票税'

--本月其它开支合计
select @本月其它开支合计 = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 <> '管理费用' and 大类 <> '财务费用' and 大类 <> '营业税金及附加' and 源 = '非业务' and 收付标志 = 2

select @业务盈亏情况+@非业务类收入合计-@管理费用开支合计-(@本月财务费用开支-@贴息费)-(@营业税金及附加合计-@开发票税合计)-@本月其它开支合计
--业务盈亏情况+非业务类收入合计－管理费用开支合计-（本月财务费用开支－贴息费）－（营业税金及附加合计－开发票税合计）－本月其它开支合计
end









CREATE proc [dbo].[过程查询_财务月报表_业务盈亏情况](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
as
begin
if exists (select * from sysobjects where name = 'Temp_财务月报表' and xtype = 'u')
begin
	drop table Temp_财务月报表
end
create table Temp_财务月报表(项目 varchar(30),金额 decimal(18,2),元 varchar(10))
declare @xm varchar(30),@je decimal(18,2)
--业务盈亏情况－预估
	select @je = (sum(case when 收付标志 = 1 then 金额小计 else 0 end) - sum(case when 收付标志 = 2 then 金额小计 else 0 end)) from 报表_业务盈亏预估表 
	where 票时间 between @日期Ge and @日期Le
	insert into Temp_财务月报表 values('业务盈亏情况-预估',@je,'元')

--其中：进口报关
	select @je = (sum(case when 收付标志 = 1 then 金额小计 else 0 end) - sum(case when 收付标志 = 2 then 金额小计 else 0 end)) from 报表_业务盈亏预估表 
	where 票时间 between @日期Ge and @日期Le and 费用实体类型 = 11 
	insert into Temp_财务月报表 values('其中：进口报关',@je,'元')

--出口报关
	insert into Temp_财务月报表 values('出口报关',0,'元')	

--内贸出港
	select @je = (sum(case when 收付标志 = 1 then 金额小计 else 0 end) - sum(case when 收付标志 = 2 then 金额小计 else 0 end)) from 报表_业务盈亏预估表 
	where 票时间 between @日期Ge and @日期Le and 费用实体类型 = 15 
	insert into Temp_财务月报表 values('内贸出港',@je,'元')	

--代开箱单
	insert into Temp_财务月报表 values('代开箱单',0,'元')

--其他业务
	select @je = (sum(case when 收付标志 = 1 then 金额小计 else 0 end) - sum(case when 收付标志 = 2 then 金额小计 else 0 end)) from 报表_业务盈亏预估表 
	where 票时间 between '2009-11-1' and '2009-11-30' and 费用实体类型 = 45 
	insert into Temp_财务月报表 values('其他业务',@je,'元')	
	
	exec ('select * from Temp_财务月报表')
	drop table Temp_财务月报表
end























CREATE proc [dbo].[过程查询_财务月报表_资金面变化情况](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
as
begin
if exists (select * from sysobjects where name = 'Temp_财务月报表_资金面变化情况' and xtype = 'u')
begin
	drop table Temp_财务月报表_资金面变化情况
end
create table Temp_财务月报表_资金面变化情况(项目 varchar(30),月初值 decimal(18,2),本月增加 decimal(18,2),本月减少 decimal(18,2))
declare @ycz decimal(18,2),@byzj decimal(18,2),@byjs decimal(18,2)
--资金面变化情况
select @ycz = sum(case when 日期 < @日期Ge and 收付标志 = 1 then 金额 else 0 end) - sum(case when 日期 < @日期Ge and 收付标志 = 2 then 金额 else 0 end), 
	   @byzj = sum(case when 日期 between @日期Ge and @日期Le and 收付标志 = 1 then 金额 else 0 end), 
	   @byjs = sum(case when 日期 between @日期Ge and @日期Le and 收付标志 = 2 then 金额 else 0 end) 
from 视图查询_资金日记帐 
insert into Temp_财务月报表_资金面变化情况 values('资金面变化情况',@ycz,@byzj,@byjs)

--现金+存款
select @ycz = sum(case when 日期 < @日期Ge and 收付标志 = 1 and 收付款方式 <> 4 then 金额 else 0 end) - sum(case when 日期 < @日期Ge and 收付标志 = 2 and 收付款方式 <> 4 then 金额 else 0 end), 
	   @byzj = sum(case when 日期 between @日期Ge and @日期Le and 收付标志 = 1 and 收付款方式 <> 4 then 金额 else 0 end), 
	   @byjs = sum(case when 日期 between @日期Ge and @日期Le and 收付标志 = 2 and 收付款方式 <> 4 then 金额 else 0 end) 
from 视图查询_资金流水明细 
insert into Temp_财务月报表_资金面变化情况 values('其中：现金+存款',@ycz,@byzj,@byjs)

--承兑汇票
select @ycz = sum(case when 日期 < @日期Ge and 收付标志 = 1 and 收付款方式 = 4 then 金额 else 0 end) - sum(case when 日期 < @日期Ge and 收付标志 = 2 and 收付款方式 = 4 then 金额 else 0 end), 
	   @byzj = sum(case when 日期 between @日期Ge and @日期Le and 收付标志 = 1 and 收付款方式 = 4 then 金额 else 0 end), 
	   @byjs = sum(case when 日期 between @日期Ge and @日期Le and 收付标志 = 2 and 收付款方式 = 4 then 金额 else 0 end) 
from 视图查询_资金流水明细 
insert into Temp_财务月报表_资金面变化情况 values('承兑汇票',@ycz,@byzj,@byjs)

--他人投资公司情况
select @ycz = sum(case when 日期 < @日期Ge then (case when 投资金额 is null then 0 else 投资金额 end) else 0 end) - sum(case when 日期 < @日期Ge then (case when 撤资金额 is null then 0 else 撤资金额 end) else 0 end),
	   @byzj = sum(case when 日期 between @日期Ge and @日期Le then 投资金额 else 0 end), 
	   @byjs = sum(case when 日期 between @日期Ge and @日期Le then 撤资金额 else 0 end) 
from 报表_非业务费用明细_他人投资公司增减明细 
insert into Temp_财务月报表_资金面变化情况 values('他人投资公司情况',@ycz,@byzj,@byjs)

--公司投资他人情况
select @ycz = sum(case when 日期 < @日期Ge then (case when 投资金额 is null then 0 else 投资金额 end) else 0 end) - sum(case when 日期 < @日期Ge then (case when 撤资金额 is null then 0 else 撤资金额 end) else 0 end), 
	   @byzj = sum(case when 日期 between @日期Ge and @日期Le then 投资金额 else 0 end), 
	   @byjs = sum(case when 日期 between @日期Ge and @日期Le then 撤资金额 else 0 end) 
from 报表_非业务费用明细_公司投资他人增减明细 
insert into Temp_财务月报表_资金面变化情况 values('公司投资他人情况',@ycz,@byzj,@byjs)

--借出款情况
select @ycz = sum(case when 日期 < @日期Ge then 金额 else 0 end), 
	   @byzj = sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 then 金额 else 0 end), 
	   @byjs = sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 then 金额 else 0 end) 
from 报表_借出款明细 
insert into Temp_财务月报表_资金面变化情况 values('借出款情况',@ycz,@byzj,@byjs)

--贷款情况
select @ycz = sum(case when 日期 < @日期Ge then 金额 else 0 end), 
	   @byzj = sum(case when 日期 between @日期Ge and @日期Le and 金额 > 0 then 金额 else 0 end), 
	   @byjs = sum(case when 日期 between @日期Ge and @日期Le and 金额 < 0 then 金额 else 0 end) 
from 报表_贷款明细 
insert into Temp_财务月报表_资金面变化情况 values('贷款情况',@ycz,@byzj,@byjs)

--固定资产情况
select @ycz = sum(月末估值) from 函数查询_报表_固定资产明细(dateadd(month,-1,@日期Ge) ,dateadd(month,-1,@日期Le))
select @byzj = sum(case when 日期 between @日期Ge and @日期Le then 购入金额 else 0 end),
	   @byjs = sum(本月折旧)  
from 函数查询_报表_固定资产明细(@日期Ge,@日期Le) 
insert into Temp_财务月报表_资金面变化情况 values('固定资产情况',@ycz,@byzj,@byjs)

select * from Temp_财务月报表_资金面变化情况
drop table Temp_财务月报表_资金面变化情况
end






-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[过程查询_固定资产折旧]
	-- Add the parameters for the stored procedure here
	@TimeGe datetime = '1980-01-01', 
	@TimeLe datetime = '2020-01-01'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT *,DATEDIFF(month,@TimeGe,@TimeLe)* 月折旧额 as 本月折旧 FROM 视图查询_固定资产折旧 A 
END














CREATE proc [dbo].[过程查询_报表_进口报关业务月报表_委托情况](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
as
begin
if exists (select * from sysobjects where name = 'Temp_进口报关业务月报表' and xtype = 'u')
begin
	drop table Temp_进口报关业务月报表
end
create table Temp_进口报关业务月报表(事项 varchar(20),状态 varchar(20),属性 varchar(10),数值 int)
declare @sx varchar(20),@zt varchar(20),@shux varchar(20),@value int
-----------------------客户委托总况-----------------------
set @sx = '客户委托总况'
set @zt = '月初遗留数'
set @shux = '票数'
--月初遗留数-总况 
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--本月委托数-总况     转关 = 1    清关 = 2
set @zt = '本月委托数'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--已放行-总况
set @zt = '已放行'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---已结束-总况
set @zt = '已结束'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---正在通关中-总况
set @zt = '正在通关中'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---正在运输中-总况
set @zt = '正在运输中'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---货未到港或单证未到-总况
set @zt = '货未到港或单证未到'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)


-----------------------客户委托转关-----------------------
set @sx = '其中：转关'
set @zt = '月初遗留数'
set @shux = '票数'
--月初遗留数-转关 
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--本月委托数-转关     转关 = 1    清关 = 2
set @zt = '本月委托数'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--已放行-转关
set @zt = '已放行'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---已结束-转关
set @zt = '已结束'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---正在通关中-转关
set @zt = '正在通关中'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---正在运输中-转关
set @zt = '正在运输中'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---货未到港或单证未到-转关
set @zt = '货未到港或单证未到'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)


-----------------------客户委托清关-----------------------
set @sx = '其中：清关'
set @zt = '月初遗留数'
set @shux = '票数'
--月初遗留数-清关 
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--本月委托数-清关     转关 = 1    清关 = 2
set @zt = '本月委托数'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--已放行-清关
set @zt = '已放行'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---已结束-清关
set @zt = '已结束'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---正在通关中-清关
set @zt = '正在通关中'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---正在运输中-清关
set @zt = '正在运输中'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---货未到港或单证未到-清关
set @zt = '货未到港或单证未到'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算客户委托总况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)


-----------------------废纸委托情况-----------------------
set @sx = '废纸委托情况'
set @zt = '月初遗留数'
set @shux = '票数'
--月初遗留数-总况 
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--本月委托数-总况     转关 = 1    清关 = 2
set @zt = '本月委托数'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--已放行-总况
set @zt = '已放行'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---已结束-总况
set @zt = '已结束'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---正在通关中-总况
set @zt = '正在通关中'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---正在运输中-总况
set @zt = '正在运输中'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---货未到港或单证未到-总况
set @zt = '货未到港或单证未到'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)



-----------------------废纸委托转关-----------------------
set @sx = '其中: 转关'
set @zt = '月初遗留数'
set @shux = '票数'
--月初遗留数-转关 
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--本月委托数-转关     转关 = 1    清关 = 2
set @zt = '本月委托数'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--已放行-转关
set @zt = '已放行'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---已结束-转关
set @zt = '已结束'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---正在通关中-转关
set @zt = '正在通关中'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---正在运输中-转关
set @zt = '正在运输中'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---货未到港或单证未到-转关
set @zt = '货未到港或单证未到'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)


-----------------------废纸委托清关-----------------------
set @sx = '其中: 清关'
set @zt = '月初遗留数'
set @shux = '票数'
--月初遗留数-转关 
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--本月委托数-转关     转关 = 1    清关 = 2
set @zt = '本月委托数'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--已放行-转关
set @zt = '已放行'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---已结束-转关
set @zt = '已结束'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---正在通关中-转关
set @zt = '正在通关中'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---正在运输中-转关
set @zt = '正在运输中'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---货未到港或单证未到-转关
set @zt = '货未到港或单证未到'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸委托情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

exec ('select * from Temp_进口报关业务月报表')
drop table Temp_进口报关业务月报表
end











CREATE proc [dbo].[过程查询_报表_修洗箱费](@对账单 varchar(20)='')
as
begin
--更新费用信息_承担
	exec 过程更新_财务_费用信息_费用承担

	select *,(select 已对账 from 函数查询_报表_修洗箱费_已对账(a.id,a.委托人)) as 已对账,
	(a.委托人承担 - (select 已对账 from 函数查询_报表_修洗箱费_已对账(a.id,a.委托人))) as 未对账 
	from 报表_修洗箱费 as a where 对账单 = @对账单
end








CREATE proc [dbo].[过程查询_报表_业务盈亏预估表_进口报关](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
as
begin
if exists (select * from sysobjects where name = 'Temp_业务盈亏预估表' and xtype = 'u')
begin
	drop table Temp_业务盈亏预估表
end
create table Temp_业务盈亏预估表(项目名称_收入 varchar(30),金额_收入 decimal(18,2),已对过账的_收入 decimal(18,2),未对账但已分离的_收入 decimal(18,2),未分离的_收入 decimal(18,2),项目名称_支出 varchar(30),金额_支出 decimal(18,2),已对过账的_支出 decimal(18,2),未对账但已分离的_支出 decimal(18,2),未分离的_支出 decimal(18,2))	
declare @金额_收入 decimal(18,2),@已对过账的_收入 decimal(18,2),@未对账但已分离的_收入 decimal(18,2),@未分离的_收入 decimal(18,2),@金额_支出 decimal(18,2),@已对过账的_支出 decimal(18,2),@未对账但已分离的_支出 decimal(18,2),@未分离的_支出 decimal(18,2)  
--本月所发生的可收入的金额_收入部分
select @金额_收入 = sum(报表收金额),@已对过账的_收入 = sum(应收金额),@未对账但已分离的_收入 = sum(case when submitted = 1 then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end),@未分离的_收入 = sum(case when submitted <> 1 or submitted is null then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 业务类型 = 11
--本月所发生的可收入的金额_支出部分
select @金额_支出 = sum(报表付金额),@已对过账的_支出 = sum(应付金额),@未对账但已分离的_支出 = sum(case when 完全标志付 = 1 then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end),@未分离的_支出 = sum(case when 完全标志付 <> 1 or 完全标志付 is null then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 业务类型 = 11

insert into Temp_业务盈亏预估表 values('本月所发生的可收入的金额',@金额_收入,@已对过账的_收入,@未对账但已分离的_收入,@未分离的_收入,'本月所发生的需付出的费用',@金额_支出,@已对过账的_支出,@未对账但已分离的_支出,@未分离的_支出)

--其中  常规费用_收入部分
select @金额_收入 = sum(报表收金额),@已对过账的_收入 = sum(应收金额),@未对账但已分离的_收入 = sum(case when submitted = 1 then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end),@未分离的_收入 = sum(case when submitted <> 1 or submitted is null then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 大类 = '业务常规' and 业务类型 = 11
--其中  常规费用_支出部分
select @金额_支出 = sum(报表付金额),@已对过账的_支出 = sum(应付金额),@未对账但已分离的_支出 = sum(case when 完全标志付 = 1 then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end),@未分离的_支出 = sum(case when 完全标志付 <> 1 or 完全标志付 is null then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 大类 = '业务常规' and 业务类型 = 11

insert into Temp_业务盈亏预估表 values('其中  常规费用',@金额_收入,@已对过账的_收入,@未对账但已分离的_收入,@未分离的_收入,'其中  常规费用',@金额_支出,@已对过账的_支出,@未对账但已分离的_支出,@未分离的_支出)

--额外费用_收入部分
select @金额_收入 = sum(报表收金额),@已对过账的_收入 = sum(应收金额),@未对账但已分离的_收入 = sum(case when submitted = 1 then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end),@未分离的_收入 = sum(case when submitted <> 1 or submitted is null then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 大类 = '业务额外' and 业务类型 = 11
--额外费用_支出部分
select @金额_支出 = sum(报表付金额),@已对过账的_支出 = sum(应付金额),@未对账但已分离的_支出 = sum(case when 完全标志付 = 1 then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end),@未分离的_支出 = sum(case when 完全标志付 <> 1 or submitted is null then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 大类 = '业务额外' and 业务类型 = 11

insert into Temp_业务盈亏预估表 values('额外费用',@金额_收入,@已对过账的_收入,@未对账但已分离的_收入,@未分离的_收入,'额外费用',@金额_支出,@已对过账的_支出,@未对账但已分离的_支出,@未分离的_支出)

exec ('select * from Temp_业务盈亏预估表')
drop table Temp_业务盈亏预估表
end
















CREATE proc [dbo].[过程查询_报表_业务盈亏预估表_内贸出港](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
as
begin
if exists (select * from sysobjects where name = 'Temp_业务盈亏预估表' and xtype = 'u')
begin
	drop table Temp_业务盈亏预估表
end
create table Temp_业务盈亏预估表(项目名称_收入 varchar(30),金额_收入 decimal(18,2),已对过账的_收入 decimal(18,2),未对账但已分离的_收入 decimal(18,2),未分离的_收入 decimal(18,2),项目名称_支出 varchar(30),金额_支出 decimal(18,2),已对过账的_支出 decimal(18,2),未对账但已分离的_支出 decimal(18,2),未分离的_支出 decimal(18,2))	
declare @金额_收入 decimal(18,2),@已对过账的_收入 decimal(18,2),@未对账但已分离的_收入 decimal(18,2),@未分离的_收入 decimal(18,2),@金额_支出 decimal(18,2),@已对过账的_支出 decimal(18,2),@未对账但已分离的_支出 decimal(18,2),@未分离的_支出 decimal(18,2)  
--本月所发生的可收入的金额_收入部分
select @金额_收入 = sum(报表收金额),@已对过账的_收入 = sum(应收金额),@未对账但已分离的_收入 = sum(case when submitted = 1 then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end),@未分离的_收入 = sum(case when submitted <> 1 or submitted is null then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 业务类型 = 15
--本月所发生的可收入的金额_支出部分
select @金额_支出 = sum(报表付金额),@已对过账的_支出 = sum(应付金额),@未对账但已分离的_支出 = sum(case when 完全标志付 = 1 then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end),@未分离的_支出 = sum(case when 完全标志付 <> 1 or 完全标志付 is null then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 业务类型 = 15

insert into Temp_业务盈亏预估表 values('本月所发生的可收入的金额',@金额_收入,@已对过账的_收入,@未对账但已分离的_收入,@未分离的_收入,'本月所发生的需付出的费用',@金额_支出,@已对过账的_支出,@未对账但已分离的_支出,@未分离的_支出)

--其中  常规费用_收入部分
select @金额_收入 = sum(报表收金额),@已对过账的_收入 = sum(应收金额),@未对账但已分离的_收入 = sum(case when submitted = 1 then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end),@未分离的_收入 = sum(case when submitted <> 1 or submitted is null then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 大类 = '业务常规' and 业务类型 = 15
--其中  常规费用_支出部分
select @金额_支出 = sum(报表付金额),@已对过账的_支出 = sum(应付金额),@未对账但已分离的_支出 = sum(case when 完全标志付 = 1 then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end),@未分离的_支出 = sum(case when 完全标志付 <> 1 or 完全标志付 is null then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 大类 = '业务常规' and 业务类型 = 15

insert into Temp_业务盈亏预估表 values('其中  常规费用',@金额_收入,@已对过账的_收入,@未对账但已分离的_收入,@未分离的_收入,'其中  常规费用',@金额_支出,@已对过账的_支出,@未对账但已分离的_支出,@未分离的_支出)

--额外费用_收入部分
select @金额_收入 = sum(报表收金额),@已对过账的_收入 = sum(应收金额),@未对账但已分离的_收入 = sum(case when submitted = 1 then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end),@未分离的_收入 = sum(case when submitted <> 1 or submitted is null then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 大类 = '业务额外' and 业务类型 = 15
--额外费用_支出部分
select @金额_支出 = sum(报表付金额),@已对过账的_支出 = sum(应付金额),@未对账但已分离的_支出 = sum(case when 完全标志付 = 1 then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end),@未分离的_支出 = sum(case when 完全标志付 <> 1 or 完全标志付 is null then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 大类 = '业务额外' and 业务类型 = 15

insert into Temp_业务盈亏预估表 values('额外费用',@金额_收入,@已对过账的_收入,@未对账但已分离的_收入,@未分离的_收入,'额外费用',@金额_支出,@已对过账的_支出,@未对账但已分离的_支出,@未分离的_支出)

exec ('select * from Temp_业务盈亏预估表')
drop table Temp_业务盈亏预估表
end













CREATE proc [dbo].[过程查询_报表_业务盈亏预估表_其他业务](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
as
begin
if exists (select * from sysobjects where name = 'Temp_业务盈亏预估表' and xtype = 'u')
begin
	drop table Temp_业务盈亏预估表
end
create table Temp_业务盈亏预估表(项目名称_收入 varchar(30),金额_收入 decimal(18,2),已对过账的_收入 decimal(18,2),未对账但已分离的_收入 decimal(18,2),未分离的_收入 decimal(18,2),项目名称_支出 varchar(30),金额_支出 decimal(18,2),已对过账的_支出 decimal(18,2),未对账但已分离的_支出 decimal(18,2),未分离的_支出 decimal(18,2))	
declare @金额_收入 decimal(18,2),@已对过账的_收入 decimal(18,2),@未对账但已分离的_收入 decimal(18,2),@未分离的_收入 decimal(18,2),@金额_支出 decimal(18,2),@已对过账的_支出 decimal(18,2),@未对账但已分离的_支出 decimal(18,2),@未分离的_支出 decimal(18,2)  
--本月所发生的可收入的金额_收入部分
select @金额_收入 = sum(报表收金额),@已对过账的_收入 = sum(应收金额),@未对账但已分离的_收入 = sum(case when submitted = 1 then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end),@未分离的_收入 = sum(case when submitted <> 1 or submitted is null then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 业务类型 = 45
--本月所发生的可收入的金额_支出部分
select @金额_支出 = sum(报表付金额),@已对过账的_支出 = sum(应付金额),@未对账但已分离的_支出 = sum(case when 完全标志付 = 1 then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end),@未分离的_支出 = sum(case when 完全标志付 <> 1 or 完全标志付 is null then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 业务类型 = 45

insert into Temp_业务盈亏预估表 values('本月所发生的可收入的金额',@金额_收入,@已对过账的_收入,@未对账但已分离的_收入,@未分离的_收入,'本月所发生的需付出的费用',@金额_支出,@已对过账的_支出,@未对账但已分离的_支出,@未分离的_支出)

--其中  常规费用_收入部分
select @金额_收入 = sum(报表收金额),@已对过账的_收入 = sum(应收金额),@未对账但已分离的_收入 = sum(case when submitted = 1 then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end),@未分离的_收入 = sum(case when submitted <> 1 or submitted is null then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 大类 = '业务常规' and 业务类型 = 45
--其中  常规费用_支出部分
select @金额_支出 = sum(报表付金额),@已对过账的_支出 = sum(应付金额),@未对账但已分离的_支出 = sum(case when 完全标志付 = 1 then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end),@未分离的_支出 = sum(case when 完全标志付 <> 1 or 完全标志付 is null then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 大类 = '业务常规' and 业务类型 = 45

insert into Temp_业务盈亏预估表 values('其中  常规费用',@金额_收入,@已对过账的_收入,@未对账但已分离的_收入,@未分离的_收入,'其中  常规费用',@金额_支出,@已对过账的_支出,@未对账但已分离的_支出,@未分离的_支出)

--额外费用_收入部分
select @金额_收入 = sum(报表收金额),@已对过账的_收入 = sum(应收金额),@未对账但已分离的_收入 = sum(case when submitted = 1 then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end),@未分离的_收入 = sum(case when submitted <> 1 or submitted is null then isnull(报表收金额,0) - isnull(应收金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 大类 = '业务额外' and 业务类型 = 45
--额外费用_支出部分
select @金额_支出 = sum(报表付金额),@已对过账的_支出 = sum(应付金额),@未对账但已分离的_支出 = sum(case when 完全标志付 = 1 then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end),@未分离的_支出 = sum(case when 完全标志付 <> 1 or 完全标志付 is null then isnull(报表付金额,0) - isnull(应付金额,0) else 0 end)
from 函数查询_报表_业务盈亏预估表(@日期Ge,@日期Le)
where 大类 = '业务额外' and 业务类型 = 45

insert into Temp_业务盈亏预估表 values('额外费用',@金额_收入,@已对过账的_收入,@未对账但已分离的_收入,@未分离的_收入,'额外费用',@金额_支出,@已对过账的_支出,@未对账但已分离的_支出,@未分离的_支出)

exec ('select * from Temp_业务盈亏预估表')
drop table Temp_业务盈亏预估表
end









CREATE proc [dbo].[过程查询_报表_滞箱费](@对账单 varchar(20)='')
as
begin
--更新费用信息_承担
	exec 过程更新_财务_费用信息_费用承担

	select *,(select 已对账 from 函数查询_报表_滞箱费_已对账(a.id,a.委托人)) as 已对账,
	(a.委托人承担 - (select 已对账 from 函数查询_报表_滞箱费_已对账(a.id,a.委托人))) as 未对账 
	from 报表_滞箱费 as a where 对账单 = @对账单

end



create proc [dbo].[过程查询_报表_资金流水_总体情况](@日期Ge datetime='2005-1-1')
as
begin
	select (sum(case when 日期 < @日期Ge and 收付款方式 <> 4 and 币制 = 'CNY' then isnull(收入金额,0) else 0 end) - sum(case when 日期 < @日期Ge and 收付款方式 <> 4 and 币制 = 'CNY' then isnull(支出金额,0) else 0 end)) 
	as 月初值_CNY,
	(sum(case when 日期 < @日期Ge and 收付款方式 <> 4 and 币制 = 'USD' then isnull(收入金额,0) else 0 end) - sum(case when 日期 < @日期Ge and 收付款方式 <> 4 and 币制 = 'USD' then isnull(支出金额,0) else 0 end)) 
	as 月初值_USD,
	(sum(case when 日期 < @日期Ge and 收付款方式 = 4 then isnull(收入金额,0) else 0 end) - sum(case when 日期 < @日期Ge and 收付款方式 = 4 then isnull(支出金额,0) else 0 end))
    as 承兑汇票月初值,
	(sum(case when 日期 < @日期Ge and 收付款方式 <> 4 and 币制 = 'CNY' then isnull(收入金额,0) else 0 end)) 
	as 现金存款本月增加,
	(sum(case when 日期 < @日期Ge and 收付款方式 <> 4 and 币制 = 'CNY' then isnull(支出金额,0) else 0 end))  
	as 现金存款本月减少,
	(sum(case when 日期 < @日期Ge and 收付款方式 = 4 then isnull(收入金额,0) else 0 end)) 
	as 承兑汇票本月增加,
	(sum(case when 日期 < @日期Ge and 收付款方式 = 4 then isnull(支出金额,0) else 0 end)) 
	as 承兑汇票本月减少
	from 报表_资金流水 

end



CREATE proc [dbo].[过程查询_报表_资金明细_总体情况](@日期Ge datetime='2005-1-1', @日期Le datetime='2009-11-30')
as
begin
	if exists (select * from sysobjects where name = 'Temp_资金明细_总体情况' and xtype = 'u')
	begin
		drop table Temp_资金明细_总体情况
	end
	create table Temp_资金明细_总体情况(手头现金_CNY decimal(18,2),手头现金_USD decimal(18,2),银行存款_CNY decimal(18,2),银行存款_USD decimal(18,2),银行存款_CNY增减 decimal(18,2),银行存款_USD增减 decimal(18,2),承兑汇票总数 int,承兑汇票金额 decimal(18,2),贴现总数 int,贴现金额 decimal(18,2))	
	declare @手头现金_CNY decimal(18,2),@手头现金_USD decimal(18,2),@银行存款_CNY decimal(18,2),@银行存款_USD decimal(18,2),@银行存款_CNY增减 decimal(18,2),@银行存款_USD增减 decimal(18,2),@承兑汇票总数 int,@承兑汇票金额 decimal(18,2),@贴现总数 int,@贴现金额 decimal(18,2)  
--手头现金
		select @手头现金_CNY = sum(case when 存取标志 = 2 and 币制 = 'CNY' then 金额 else 0 end) - sum(case when 存取标志 = 1 and 币制 = 'CNY' then 金额 else 0 end),
			   @手头现金_USD = sum(case when 存取标志 = 2 and 币制 = 'USD' then 金额 else 0 end) - sum(case when 存取标志 = 1 and 币制 = 'USD' then 金额 else 0 end)
		from 视图查询_现金日记帐 
		where 日期 <= @日期Le

--银行存款
		select @银行存款_CNY = isnull(sum(月初值),0)+ isnull(sum(本月增减),0),@银行存款_CNY增减 = isnull(sum(本月增减),0) 
		from 函数查询_报表_资金明细_银行存款(@日期Ge,@日期Le)
		where 币制 = 'CNY'
		
		select @银行存款_USD = isnull(sum(月初值),0)+ isnull(sum(本月增减),0),@银行存款_USD增减 = isnull(sum(本月增减),0) 
		from 函数查询_报表_资金明细_银行存款(@日期Ge,@日期Le)
		where 币制 = 'USD'

--承兑汇票 
		select @承兑汇票总数 = count(所属银行),@承兑汇票金额 = sum(金额),@贴现总数 = sum(case when 托收贴现 = 2 then 1 else 0 end),@贴现金额 = sum(case when 托收贴现 = 2 then 金额 else 0 end) 
		from 函数查询_报表_资金明细_承兑汇票(@日期Le)
		
		insert into Temp_资金明细_总体情况 values(@手头现金_CNY,@手头现金_USD,@银行存款_CNY,@银行存款_USD,@银行存款_CNY增减,@银行存款_USD增减,@承兑汇票总数,@承兑汇票金额,@贴现总数,@贴现金额)
		exec ('select * from Temp_资金明细_总体情况')
		drop table Temp_资金明细_总体情况

end








CREATE proc [dbo].[过程查询_财务月报表_财务费用开支合计](@日期Ge datetime='2008-10-1',@日期Le datetime='2008-10-31')
as
begin
if exists (select * from sysobjects where name = 'Temp_财务月报表' and xtype = 'u')
begin
	drop table Temp_财务月报表
end
create table Temp_财务月报表(费用项 varchar(30),本月 decimal(18,2),上月 decimal(18,2))
declare @by decimal(18,2),@sy decimal(18,2)
--财务费用开支合计
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 like '%财务费用%' 
insert into Temp_财务月报表 values('财务费用开支合计',@by,@sy)

--贴息费
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 = '财务费用' and 小类 = '贴息' 
insert into Temp_财务月报表 values('其中：贴息费',@by,@sy)

--贷款利息
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 = '财务费用' and 小类 = '利息' 
insert into Temp_财务月报表 values('贷款利息',@by,@sy)

--营业税金及附加合计
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 = '营业税金及附加' 
insert into Temp_财务月报表 values('营业税金及附加合计',@by,@sy)

--开发票税
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 = '营业税金及附加' and 小类 = '开票税'
insert into Temp_财务月报表 values('其中：开发票税合计',@by,@sy)

select * from Temp_财务月报表
drop table Temp_财务月报表
end










CREATE proc [dbo].[过程查询_财务月报表_待确认业务](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
as
begin
if exists (select * from sysobjects where name = 'Temp_财务月报表' and xtype = 'u')
begin
	drop table Temp_财务月报表
end
create table Temp_财务月报表(项目 varchar(30),金额 decimal(18,2))
declare @je decimal(18,2)
--待确认业务预计收款余额
	select @je = sum(月末前待确认业务_常规费用) + sum(月末前待确认业务_额外费用) from 函数查询_报表_业务应收款月报表_总表_月末前待确认业务(@日期Ge,@日期Le)
	insert into Temp_财务月报表 values('待确认业务预计收款余额',@je)

--其中：常规费用
	select @je = sum(月末前待确认业务_常规费用) from 函数查询_报表_业务应收款月报表_总表_月末前待确认业务(@日期Ge,@日期Le)
	insert into Temp_财务月报表 values('其中：常规费用',@je)

--额外费用
	select @je = sum(月末前待确认业务_额外费用) from 函数查询_报表_业务应收款月报表_总表_月末前待确认业务(@日期Ge,@日期Le)
	insert into Temp_财务月报表 values('额外费用',@je)


--待确认业务预计付款余额
	select @je = sum(月末前待确认业务_常规费用) + sum(月末前待确认业务_额外费用) from 函数查询_报表_业务应付款月报表_总表_月末前待确认业务(@日期Ge,@日期Le)
	insert into Temp_财务月报表 values('待确认业务预计付款余额',@je)

--其中：常规费用
	select @je = sum(月末前待确认业务_常规费用) from 函数查询_报表_业务应付款月报表_总表_月末前待确认业务(@日期Ge,@日期Le)
	insert into Temp_财务月报表 values('其中：常规费用',@je)

--额外费用
	select @je = sum(月末前待确认业务_额外费用) from 函数查询_报表_业务应付款月报表_总表_月末前待确认业务(@日期Ge,@日期Le)
	insert into Temp_财务月报表 values('额外费用',@je)

exec ('select * from Temp_财务月报表')
drop table Temp_财务月报表

end













CREATE proc [dbo].[过程查询_财务月报表_非业务类收入合计](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
as
begin
if exists (select * from sysobjects where name = 'Temp_财务月报表' and xtype = 'u')
begin
	drop table Temp_财务月报表
end
create table Temp_财务月报表(费用项 varchar(30),本月 decimal(18,2),上月 decimal(18,2))
declare @by decimal(18,2),@sy decimal(18,2)
--非业务类收入合计
select @by = sum(case when 入账日期 between @日期Ge and @日期Le and 收付标志 = 1 then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) and 收付标志 = 1 then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 源 = '非业务' 
insert into Temp_财务月报表 values('非业务类收入合计',@by,@sy)

--固定资产出租收益
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 源 = '非业务' and 小类 = '资产出租'
insert into Temp_财务月报表 values('其中：固定资产出租收益',@by,@sy)

--借出款利息收入
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 源 = '非业务' and 小类 = '利息收入'
insert into Temp_财务月报表 values('借出款利息收入',@by,@sy)

--投资收益
select @by = sum(case when 入账日期 between @日期Ge and @日期Le and 收付标志 = 1 then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) and 收付标志 = 1 then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 源 = '非业务' and 小类 = '投资'
insert into Temp_财务月报表 values('投资收益',@by,@sy)


select * from Temp_财务月报表
drop table Temp_财务月报表
end






















CREATE proc [dbo].[过程查询_财务月报表_管理费用开支合计](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
as
begin
if exists (select * from sysobjects where name = 'Temp_财务月报表' and xtype = 'u')
begin
	drop table Temp_财务月报表
end
create table Temp_财务月报表(费用项 varchar(30),本月 decimal(18,2),上月 decimal(18,2))
declare @by decimal(18,2),@sy decimal(18,2)
--管理费用开支合计
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 like '%管理费用%' 
insert into Temp_财务月报表 values('管理费用开支合计',@by,@sy)

--工资、奖金、三金
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 = '管理费用' and 小类 = '工资及福利' 
insert into Temp_财务月报表 values('其中：工资、奖金、三金',@by,@sy)

--交际费
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 = '管理费用' and 小类 = '业务招待费' 
insert into Temp_财务月报表 values('交际费',@by,@sy)

--通讯、办公费用
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 = '管理费用' and 小类 in ('办公费','通讯费')
insert into Temp_财务月报表 values('通讯、办公费用',@by,@sy)

--房租费
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 = '管理费用' and 小类 = '房租费'
insert into Temp_财务月报表 values('房租费',@by,@sy)

--本月处理坏账损失
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 = '管理费用' and 小类 = '坏账损失'
insert into Temp_财务月报表 values('本月处理坏账损失',@by,@sy)

--固定资产折旧
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 = '管理费用' and 小类 = '资产折旧'
insert into Temp_财务月报表 values('固定资产折旧',@by,@sy)

--本月实付佣金合计
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 = '管理费用' and 小类 = '佣金'
insert into Temp_财务月报表 values('本月实付佣金合计',@by,@sy)

--本月其他开支合计
select @by = sum(case when 入账日期 between @日期Ge and @日期Le then 金额 else 0 end),
	   @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) then 金额 else 0 end) 
from 视图查询月报表_非业务及业务常规费用明细
WHERE 大类 <> '管理费用' and 大类 <> '财务费用' and 大类 <> '营业税金及附加' and 源 = '非业务' and 收付标志 = 2
insert into Temp_财务月报表 values('本月其他开支合计',@by,@sy)

--select @by = sum(case when 入账日期 between @日期Ge and @日期Le and 收付标志 = 1 and 源 = '非业务' then 金额 else 0 end)- 
--sum(case when 入账日期 between @日期Ge and @日期Le and 大类 like '%管理费用%' then 金额 else 0 end)-
--sum(case when 入账日期 between @日期Ge and @日期Le and 大类 like '%财务费用%' then 金额 else 0 end)-
--sum(case when 入账日期 between @日期Ge and @日期Le and 大类 = '营业税金及附加' then 金额 else 0 end) 
--from 视图查询月报表_非业务及业务常规费用明细
--
--select @sy = sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) and 收付标志 = 1 and 源 = '非业务' then 金额 else 0 end)- 
--sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) and 大类 like '%管理费用%' then 金额 else 0 end)-
--sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) and 大类 like '%财务费用%' then 金额 else 0 end)-
--sum(case when 入账日期 between dateadd(month,-1,@日期Ge) and dateadd(month,-1,@日期Le) and 大类 = '营业税金及附加' then 金额 else 0 end) 
--from 视图查询月报表_非业务及业务常规费用明细
--
--insert into Temp_财务月报表 values('本月其他开支合计',@by,@sy)

select * from Temp_财务月报表
drop table Temp_财务月报表
end














CREATE proc [dbo].[过程查询_财务月报表_业务应收应付款](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
as
begin
if exists (select * from sysobjects where name = 'Temp_财务月报表_资金面变化情况' and xtype = 'u')
begin
	drop table Temp_财务月报表
end
create table Temp_财务月报表(项目 varchar(30),月初值 decimal(18,2),本月增加 decimal(18,2),本月减少 decimal(18,2))
declare @ycz decimal(18,2),@byzj decimal(18,2),@byjs decimal(18,2)
--业务应收款情况
select @ycz = sum(月初_常规费用) + sum(月初_额外费用),
	   @byzj = sum(当月增加_常规费用) + sum(当月增加_额外费用),
	   @byjs = sum(当月减少_常规费用) + sum(当月减少_额外费用)
	   --sum(月末_常规费用) + sum(月末_额外费用)
from 函数查询_报表_业务应收款月报表_总表_业务应收款(@日期Ge,@日期Le)
insert into Temp_财务月报表 values('业务应收款情况',@ycz,@byzj,@byjs)

--其中：常规费用
select @ycz = sum(月初_常规费用),@byzj = sum(当月增加_常规费用),@byjs = sum(当月减少_常规费用)--,sum(月末_常规费用) 
from 函数查询_报表_业务应收款月报表_总表_业务应收款(@日期Ge,@日期Le)
insert into Temp_财务月报表 values('其中：常规费用',@ycz,@byzj,@byjs)

--额外费用
select @ycz = sum(月初_额外费用),@byzj = sum(当月增加_额外费用),@byjs = sum(当月减少_额外费用)--,sum(月末_额外费用) 
from 函数查询_报表_业务应收款月报表_总表_业务应收款(@日期Ge,@日期Le)
insert into Temp_财务月报表 values('额外费用',@ycz,@byzj,@byjs)

--业务应付款情况
select @ycz = sum(月初_常规费用) + sum(月初_额外费用),
	   @byzj = sum(当月增加_常规费用) + sum(当月增加_额外费用),
	   @byjs = sum(当月减少_常规费用) + sum(当月减少_额外费用)
	   --sum(月末_常规费用) + sum(月末_额外费用)
from 函数查询_报表_业务应付款月报表_总表_业务应付款(@日期Ge,@日期Le)
insert into Temp_财务月报表 values('业务应付款情况',@ycz,@byzj,@byjs)

--其中：常规费用
select @ycz = sum(月初_常规费用),@byzj = sum(当月增加_常规费用),@byjs = sum(当月减少_常规费用)--,sum(月末_常规费用) 
from 函数查询_报表_业务应付款月报表_总表_业务应付款(@日期Ge,@日期Le)
insert into Temp_财务月报表 values('其中：常规费用',@ycz,@byzj,@byjs)

--额外费用
select @ycz = sum(月初_额外费用),@byzj = sum(当月增加_额外费用),@byjs = sum(当月减少_额外费用)--,sum(月末_额外费用) 
from 函数查询_报表_业务应付款月报表_总表_业务应付款(@日期Ge,@日期Le)
insert into Temp_财务月报表 values('额外费用',@ycz,@byzj,@byjs)

--其它应收款情况
select @ycz = sum(月初_常规费用) + sum(月初_额外费用),
	   @byzj = sum(当月增加_常规费用) + sum(当月增加_额外费用),
	   @byjs = sum(当月减少_常规费用) + sum(当月减少_额外费用)
	   --sum(月末_常规费用) + sum(月末_额外费用)
from 函数查询_报表_业务应收款月报表_其它_业务应收款(@日期Ge,@日期Le)
insert into Temp_财务月报表 values('其它应收款情况',@ycz,@byzj,@byjs)

--其它应付款情况
select @ycz = sum(月初_常规费用) + sum(月初_额外费用),
	   @byzj = sum(当月增加_常规费用) + sum(当月增加_额外费用),
	   @byjs = sum(当月减少_常规费用) + sum(当月减少_额外费用)
	   --sum(月末_常规费用) + sum(月末_额外费用)
from 函数查询_报表_业务应付款月报表_其它_业务应付款(@日期Ge,@日期Le)
insert into Temp_财务月报表 values('其它应付款情况',@ycz,@byzj,@byjs)

exec ('select * from Temp_财务月报表')
drop table Temp_财务月报表
end











CREATE proc [dbo].[过程查询_报表_进口报关业务月报表_计算废纸委托情况](@sx varchar(20),@zt varchar(20),@shux varchar(10),@value int output,@日期Ge datetime,@日期Le datetime)
as
begin
----------------------------废纸委托情况-----------------------
	if @sx = '废纸委托情况'
	begin
	--月初遗留数-总况
		if @zt = '月初遗留数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((委托时间 < @日期Ge and 承运标志 = 0 and (放行时间 is null or 放行时间 >= @日期Ge)) or (委托时间 < @日期Ge and 承运标志 = 1 and (结束时间 is null or 结束时间 >= @日期Ge))) and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((委托时间 < @日期Ge and 承运标志 = 0 and (放行时间 is null or 放行时间 >= @日期Ge)) or (委托时间 < @日期Ge and 承运标志 = 1 and (结束时间 is null or 结束时间 >= @日期Ge))) and 货物类别 like '%废纸%'
			end
		end
	--本月委托数-总况
		if @zt = '本月委托数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 委托时间 between @日期Ge and @日期Le and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 委托时间 between @日期Ge and @日期Le and 货物类别 like '%废纸%'
			end
		end
	--已放行-总况
		if @zt = '已放行'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and 货物类别 like '%废纸%'
			end
		end
	--已结束-总况
		if @zt = '已结束'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (承运标志 = 0 and 放行时间 between @日期Ge and @日期Le) or (承运标志 = 1 and 结束时间 between @日期Ge and @日期Le) and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (承运标志 = 0 and 放行时间 between @日期Ge and @日期Le) or (承运标志 = 1 and 结束时间 between @日期Ge and @日期Le) and 货物类别 like '%废纸%'
			end
		end
	--正在通关中-总况
		if @zt = '正在通关中'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 <= @日期Le and (放行时间 is null or 放行时间 > @日期Le) and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 <= @日期Le and (放行时间 is null or 放行时间 > @日期Le) and 货物类别 like '%废纸%'
			end
		end
	--正在运输中-总况
		if @zt = '正在运输中'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and (结束时间 > @日期Le or 结束时间 is null ) and 承运标志 = 1 and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and (结束时间 > @日期Le or 结束时间 is null ) and 承运标志 = 1 and 货物类别 like '%废纸%'
			end
		end
	--货未到港或单证未到-总况
		if @zt = '货未到港或单证未到'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 is null or 报关时间 > @日期Le and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 is null or 报关时间 > @日期Le and 货物类别 like '%废纸%'
			end
		end
	end


----------------------------废纸委托转关-----------------------
	if @sx = '其中: 转关'
	begin
	--月初遗留数-转关
		if @zt = '月初遗留数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((委托时间 < @日期Ge and 承运标志 = 0 and (放行时间 is null or 放行时间 >= @日期Ge)) or (委托时间 < @日期Ge and 承运标志 = 1 and (结束时间 is null or 结束时间 >= @日期Ge))) and 通关类别 = 1 and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((委托时间 < @日期Ge and 承运标志 = 0 and (放行时间 is null or 放行时间 >= @日期Ge)) or (委托时间 < @日期Ge and 承运标志 = 1 and (结束时间 is null or 结束时间 >= @日期Ge))) and 通关类别 = 1 and 货物类别 like '%废纸%'
			end
		end
	--本月委托数-转关
		if @zt = '本月委托数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 委托时间 between @日期Ge and @日期Le and 通关类别 = 1 and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 委托时间 between @日期Ge and @日期Le and 通关类别 = 1 and 货物类别 like '%废纸%'
			end
		end
	--已放行-转关
		if @zt = '已放行'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and 通关类别 = 1 and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and 通关类别 = 1 and 货物类别 like '%废纸%'
			end
		end
	--已结束-转关
		if @zt = '已结束'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((承运标志 = 0 and 放行时间 between @日期Ge and @日期Le) or (承运标志 = 1 and 结束时间 between @日期Ge and @日期Le)) and 通关类别 = 1 and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((承运标志 = 0 and 放行时间 between @日期Ge and @日期Le) or (承运标志 = 1 and 结束时间 between @日期Ge and @日期Le)) and 通关类别 = 1 and 货物类别 like '%废纸%'
			end
		end
	--正在通关中-转关
		if @zt = '正在通关中'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 <= @日期Le and (放行时间 is null or 放行时间 > @日期Le) and 通关类别 = 1 and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 <= @日期Le and (放行时间 is null or 放行时间 > @日期Le) and 通关类别 = 1 and 货物类别 like '%废纸%'
			end
		end
	--正在运输中-转关
		if @zt = '正在运输中'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and (结束时间 > @日期Le or 结束时间 is null ) and 承运标志 = 1 and 通关类别 = 1 and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and (结束时间 > @日期Le or 结束时间 is null ) and 承运标志 = 1 and 通关类别 = 1 and 货物类别 like '%废纸%'
			end
		end
	--货未到港或单证未到-转关
		if @zt = '货未到港或单证未到'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 is null or 报关时间 > @日期Le and 通关类别 = 1 and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 is null or 报关时间 > @日期Le and 通关类别 = 1 and 货物类别 like '%废纸%'
			end
		end
	end

---------------------------废纸委托清关----------------------
	if @sx = '其中: 清关'
	begin
	--月初遗留数-清关
		if @zt = '月初遗留数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((委托时间 < @日期Ge and 承运标志 = 0 and (放行时间 is null or 放行时间 >= @日期Ge)) or (委托时间 < @日期Ge and 承运标志 = 1 and (结束时间 is null or 结束时间 >= @日期Ge))) and 通关类别 = 2 and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((委托时间 < @日期Ge and 承运标志 = 0 and (放行时间 is null or 放行时间 >= @日期Ge)) or (委托时间 < @日期Ge and 承运标志 = 1 and (结束时间 is null or 结束时间 >= @日期Ge))) and 通关类别 = 2 and 货物类别 like '%废纸%'
			end
		end
	--本月委托数-清关
		if @zt = '本月委托数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 委托时间 between @日期Ge and @日期Le and 通关类别 = 2 and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 委托时间 between @日期Ge and @日期Le and 通关类别 = 2 and 货物类别 like '%废纸%'
			end
		end
	--已放行-清关
		if @zt = '已放行'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and 通关类别 = 2 and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and 通关类别 = 2 and 货物类别 like '%废纸%'
			end
		end
	--已结束-清关
		if @zt = '已结束'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((承运标志 = 0 and 放行时间 between @日期Ge and @日期Le) or (承运标志 = 1 and 结束时间 between @日期Ge and @日期Le)) and 通关类别 = 2 and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((承运标志 = 0 and 放行时间 between @日期Ge and @日期Le) or (承运标志 = 1 and 结束时间 between @日期Ge and @日期Le)) and 通关类别 = 2 and 货物类别 like '%废纸%'
			end
		end
	--正在通关中-清关
		if @zt = '正在通关中'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 <= @日期Le and (放行时间 is null or 放行时间 > @日期Le) and 通关类别 = 2 and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 <= @日期Le and (放行时间 is null or 放行时间 > @日期Le) and 通关类别 = 2 and 货物类别 like '%废纸%'
			end
		end
	--正在运输中-清关
		if @zt = '正在运输中'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and (结束时间 > @日期Le or 结束时间 is null ) and 承运标志 = 1 and 通关类别 = 2 and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and (结束时间 > @日期Le or 结束时间 is null ) and 承运标志 = 1 and 通关类别 = 2 and 货物类别 like '%废纸%'
			end
		end
	--货未到港或单证未到-清关
		if @zt = '货未到港或单证未到'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 is null or 报关时间 > @日期Le and 通关类别 = 2 and 货物类别 like '%废纸%'
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 is null or 报关时间 > @日期Le and 通关类别 = 2 and 货物类别 like '%废纸%'
			end
		end
	end
end






















CREATE proc [dbo].[过程查询_报表_进口报关业务月报表_计算客户委托总况](@sx varchar(20),@zt varchar(20),@shux varchar(10),@value int output,@日期Ge datetime,@日期Le datetime)
as
--------------------------------客户委托总况----------------------
begin
	if @sx = '客户委托总况'
	begin
		--月初遗留数-总况
		if @zt = '月初遗留数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (委托时间 < @日期Ge and 承运标志 = 0 and (放行时间 is null or 放行时间 >= @日期Ge)) or (委托时间 < @日期Ge and 承运标志 = 1 and (结束时间 is null or 结束时间 >= @日期Ge))
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (委托时间 < @日期Ge and 承运标志 = 0 and (放行时间 is null or 放行时间 >= @日期Ge)) or (委托时间 < @日期Ge and 承运标志 = 1 and (结束时间 is null or 结束时间 >= @日期Ge))
			end
		end
	--本月委托数-总况
		if @zt = '本月委托数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 委托时间 between @日期Ge and @日期Le
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 委托时间 between @日期Ge and @日期Le
			end
		end
	--已放行-总况
		if @zt = '已放行'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le 
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le
			end
		end
	--已结束-总况
		if @zt = '已结束'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (承运标志 = 0 and 放行时间 between @日期Ge and @日期Le) or (承运标志 = 1 and 结束时间 between @日期Ge and @日期Le)
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (承运标志 = 0 and 放行时间 between @日期Ge and @日期Le) or (承运标志 = 1 and 结束时间 between @日期Ge and @日期Le)
			end
		end
	--正在通关中-总况
		if @zt = '正在通关中'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 <= @日期Le and (放行时间 is null or 放行时间 > @日期Le)
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 <= @日期Le and (放行时间 is null or 放行时间 > @日期Le)
			end
		end
	--正在运输中-总况
		if @zt = '正在运输中'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and (结束时间 > @日期Le or 结束时间 is null ) and 承运标志 = 1
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and (结束时间 > @日期Le or 结束时间 is null ) and 承运标志 = 1
			end
		end
	--货未到港或单证未到-总况
		if @zt = '货未到港或单证未到'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 is null or 报关时间 > @日期Le
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 报关时间 is null or 报关时间 > @日期Le
			end
		end
	end

---------------------------------客户委托转关--------------------------
	if @sx = '其中：转关'
	begin
	--月初遗留数-转关
		if @zt = '月初遗留数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((委托时间 < @日期Ge and 承运标志 = 0 and (放行时间 is null or 放行时间 >= @日期Ge)) or (委托时间 < @日期Ge and 承运标志 = 1 and (结束时间 is null or 结束时间 >= @日期Ge))) and 通关类别 = 1
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((委托时间 < @日期Ge and 承运标志 = 0 and (放行时间 is null or 放行时间 >= @日期Ge)) or (委托时间 < @日期Ge and 承运标志 = 1 and (结束时间 is null or 结束时间 >= @日期Ge))) and 通关类别 = 1
			end
		end
	--本月委托数-转关
		if @zt = '本月委托数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 委托时间 between @日期Ge and @日期Le and 通关类别 = 1
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 委托时间 between @日期Ge and @日期Le and 通关类别 = 1
			end
		end
	--已放行-转关
		if @zt = '已放行'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and 通关类别 = 1
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and 通关类别 = 1
			end
		end
	--已结束-转关
		if @zt = '已结束'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((承运标志 = 0 and 放行时间 between @日期Ge and @日期Le) or (承运标志 = 1 and 结束时间 between @日期Ge and @日期Le)) and 通关类别 = 1
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((承运标志 = 0 and 放行时间 between @日期Ge and @日期Le) or (承运标志 = 1 and 结束时间 between @日期Ge and @日期Le)) and 通关类别 = 1
			end
		end
	--正在通关中-转关
		if @zt = '正在通关中'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (报关时间 <= @日期Le and (放行时间 > @日期Le or 放行时间 is null )) and 通关类别 = 1
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (报关时间 <= @日期Le and (放行时间 > @日期Le or 放行时间 is null )) and 通关类别 = 1
			end
		end
	--正在运输中-转关
		if @zt = '正在运输中'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (放行时间 <= @日期Le and (结束时间 > @日期Le or 结束时间 is null ) and 承运标志 = 1) and 通关类别 = 1
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (放行时间 <= @日期Le and (结束时间 > @日期Le or 结束时间 is null ) and 承运标志 = 1) and 通关类别 = 1
			end
		end
	--货未到港或单证未到-转关
		if @zt = '货未到港或单证未到'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (报关时间 is null or 报关时间 > @日期Le) and 通关类别 = 1
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (报关时间 is null or 报关时间 > @日期Le) and 通关类别 = 1
			end
		end
	end

------------------------------客户委托清关--------------------------
	if @sx = '其中：清关'
	begin
--月初遗留数-清关
		if @zt = '月初遗留数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((委托时间 < @日期Ge and 承运标志 = 0 and (放行时间 is null or 放行时间 >= @日期Ge)) or (委托时间 < @日期Ge and 承运标志 = 1 and (结束时间 is null or 结束时间 >= @日期Ge))) and 通关类别 = 2
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((委托时间 < @日期Ge and 承运标志 = 0 and (放行时间 is null or 放行时间 >= @日期Ge)) or (委托时间 < @日期Ge and 承运标志 = 1 and (结束时间 is null or 结束时间 >= @日期Ge))) and 通关类别 = 2
			end
		end
	--本月委托数-清关
		if @zt = '本月委托数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 委托时间 between @日期Ge and @日期Le and 通关类别 = 2
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 委托时间 between @日期Ge and @日期Le and 通关类别 = 2
			end
		end
	--已放行-清关
		if @zt = '已放行'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and 通关类别 = 2
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 放行时间 <= @日期Le and 通关类别 = 2
			end
		end
	--已结束-清关
		if @zt = '已结束'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((承运标志 = 0 and 放行时间 between @日期Ge and @日期Le) or (承运标志 = 1 and 结束时间 between @日期Ge and @日期Le)) and 通关类别 = 2
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where ((承运标志 = 0 and 放行时间 between @日期Ge and @日期Le) or (承运标志 = 1 and 结束时间 between @日期Ge and @日期Le)) and 通关类别 = 2
			end
		end
	--正在通关中-清关
		if @zt = '正在通关中'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (报关时间 <= @日期Le and (放行时间 > @日期Le or 放行时间 is null )) and 通关类别 = 2
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (报关时间 <= @日期Le and (放行时间 > @日期Le or 放行时间 is null )) and 通关类别 = 2
			end
		end
	--正在运输中-清关
		if @zt = '正在运输中'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (放行时间 <= @日期Le and (结束时间 > @日期Le or 结束时间 is null ) and 承运标志 = 1) and 通关类别 = 2
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (放行时间 <= @日期Le and (结束时间 > @日期Le or 结束时间 is null ) and 承运标志 = 1) and 通关类别 = 2
			end
		end
	--货未到港或单证未到-清关
		if @zt = '货未到港或单证未到'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (报关时间 is null or 报关时间 > @日期Le) and 通关类别 = 2
			end

			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (报关时间 is null or 报关时间 > @日期Le) and 通关类别 = 2
			end
		end
	end
	end















CREATE proc [dbo].[过程更新_财务_费用信息_单条](@id varchar(4000))
as
begin
--收款时间
update 财务_费用信息 set 收款创建时间 = (select 收款创建时间 from 函数更新_财务_费用信息_收款时间(a.id)),
					     收款修改时间 = (select 收款修改时间 from 函数更新_财务_费用信息_收款时间(a.id)),
--付款时间	
						 付款创建时间 = (select 付款创建时间 from 函数更新_财务_费用信息_付款时间(a.id)),
					     付款修改时间 = (select 付款修改时间 from 函数更新_财务_费用信息_付款时间(a.id)),
--凭证号
						 凭证号 = (select (case when 凭证号 = '' then Null else 凭证号 end) from 函数更新_财务_费用信息_凭证号(a.id)),
--对账单号	
						 对账单号 = (select (case when 对账单号 = '' then Null else 对账单号 end) from 函数更新_财务_费用信息_对账单号(a.id)),			
--创建天数
						 创建天数 = (datediff(day,(case when a.付款创建时间 > a.收款创建时间 then a.收款创建时间 
														  when a.付款创建时间 < a.收款创建时间 then a.付款创建时间 
															when a.付款创建时间 is null or a.收款创建时间 is null then null end),getdate())),
--收款对账凭证状态
						 收款对账凭证状态 = (select 收款对账凭证状态 from 函数更新_财务_费用信息_收款对账凭证状态(a.id)),
--付款对账凭证状态
						 付款对账凭证状态 = (select 付款对账凭证状态 from 函数更新_财务_费用信息_付款对账凭证状态(a.id)),
--警示状态

--填全状态				
						 填全状态 = (select (case when 填全状态 = 0 then 'True' else 'False' end) from 函数更新_财务_费用信息_填全状态(a.id))

from 财务_费用信息 as a --left join dbo.参数备案_费用项 as b on a.费用项 = b.编号
where charindex(','+ltrim(a.id)+',',','+@id+',')>0


--委托人承担
update 财务_费用信息 set 委托人承担 = (select 委托人承担 from 函数更新_财务_费用信息_委托人承担(a.id)),
--车队承担
						 车队承担 = (select 车队承担 from 函数更新_财务_费用信息_车队承担(a.id)),
--对外付款
						 对外付款 = (select 对外付款 from 函数更新_财务_费用信息_对外付款(a.id)),
--自己承担
						 自己承担 = (select isnull(对外付款,0) - isnull(委托人承担,0) - isnull(车队承担,0))
from 财务_费用信息 as a 
where charindex(','+ltrim(a.id)+',',','+@id+',')>0

end




CREATE proc [dbo].[过程查询_报表_进口报关业务月报表_计算全部异常操作情况](@sx varchar(20),@zt varchar(20),@shux varchar(10),@value int output,@日期Ge datetime,@日期Le datetime)
as
begin
---------------------全部异常操作情况--------------------------
	if @sx = '全部异常操作情况'
	begin
		if @zt = '总合计数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 单证晚到 > 0 or 通关天数 > 5 or 承运超期 > 0
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 单证晚到 > 0 or 通关天数 > 5 or 承运超期 > 0
			end
		end

		if @zt = '单证晚到超过4天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 单证晚到 > 4 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 单证晚到 > 4
			end
		end

		if @zt = '通关超过5天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 通关天数 > 5 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 通关天数 > 5
			end
		end

		if @zt = '承运超过4天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 承运超期 > 4 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 承运超期 > 4
			end
		end

		if @zt = '产生疏港'
		begin
			if @shux = '票数'
			begin 
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where datediff(day,到港时间,开始时间) > 15 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where datediff(day,到港时间,开始时间) > 15 
			end
		end

		if @zt = '产生滞箱'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 滞箱费减免标志 = 'False' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 滞箱费减免标志 = 'False'
			end
		end
	end


---------------------全部异常转关--------------------------
	if @sx = '其中：转关'
	begin
		if @zt = '总合计数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (单证晚到 > 0 or 通关天数 > 5 or 承运超期 > 0) and 通关类别 = 1
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (单证晚到 > 0 or 通关天数 > 5 or 承运超期 > 0) and 通关类别 = 1
			end
		end

		if @zt = '单证晚到超过4天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 单证晚到 > 4 and 通关类别 = 1 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 单证晚到 > 4 and 通关类别 = 1
			end
		end

		if @zt = '通关超过5天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 通关天数 > 5 and 通关类别 = 1 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 通关天数 > 5 and 通关类别 = 1
			end
		end

		if @zt = '承运超过4天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 承运超期 > 4 and 通关类别 = 1 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 承运超期 > 4 and 通关类别 = 1
			end
		end

		if @zt = '产生疏港'
		begin
			if @shux = '票数'
			begin 
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where datediff(day,到港时间,开始时间) > 15 and 通关类别 = 1 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where datediff(day,到港时间,开始时间) > 15 and 通关类别 = 1 
			end
		end

		if @zt = '产生滞箱'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 滞箱费减免标志 = 'False' and 通关类别 = 1 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 滞箱费减免标志 = 'False' and 通关类别 = 1
			end
		end
	end


---------------------全部异常清关--------------------------
	if @sx = '其中：清关'
	begin
		if @zt = '总合计数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (单证晚到 > 0 or 通关天数 > 5 or 承运超期 > 0) and 通关类别 = 2
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (单证晚到 > 0 or 通关天数 > 5 or 承运超期 > 0) and 通关类别 = 2
			end
		end

		if @zt = '单证晚到超过4天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 单证晚到 > 4 and 通关类别 = 2 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 单证晚到 > 4 and 通关类别 = 2
			end
		end

		if @zt = '通关超过5天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 通关天数 > 5 and 通关类别 = 2 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 通关天数 > 5 and 通关类别 = 2
			end
		end

		if @zt = '承运超过4天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 承运超期 > 4 and 通关类别 = 2 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 承运超期 > 4 and 通关类别 = 2
			end
		end

		if @zt = '产生疏港'
		begin
			if @shux = '票数'
			begin 
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where datediff(day,到港时间,开始时间) > 15 and 通关类别 = 2 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where datediff(day,到港时间,开始时间) > 15 and 通关类别 = 2
			end
		end

		if @zt = '产生滞箱'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 滞箱费减免标志 = 'False' and 通关类别 = 2 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 滞箱费减免标志 = 'False' and 通关类别 = 2
			end
		end
	end




end











CREATE proc [dbo].[过程查询_报表_进口报关业务月报表_操作情况](@日期Ge datetime='2005-1-1',@日期Le datetime='2020-12-31')
as
begin
if exists (select * from sysobjects where name = 'Temp_进口报关业务月报表' and xtype = 'u')
begin
	drop table Temp_进口报关业务月报表
end
create table Temp_进口报关业务月报表(事项 varchar(20),状态 varchar(20),属性 varchar(10),数值 int)
declare @sx varchar(20),@zt varchar(20),@shux varchar(20),@value int
-----------------------全部异常操作情况-----------------------
set @sx = '全部异常操作情况'
set @zt = '总合计数'
set @shux = '票数'
--总合计数 
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--单证晚到超过4天     转关 = 1    清关 = 2
set @zt = '单证晚到超过4天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--通关超过5天
set @zt = '通关超过5天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---承运超过4天
set @zt = '承运超过4天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---产生疏港
set @zt = '产生疏港'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---产生滞箱
set @zt = '产生滞箱'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---其他异常
--set @zt = '其他异常'
--set @shux = '票数'
--EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
--insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)
--
--set @shux = '箱量'
--EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
--insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)


-----------------------全部异常转关-----------------------
set @sx = '其中：转关'
set @zt = '总合计数'
set @shux = '票数'
--总合计数 
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--单证晚到超过4天     转关 = 1    清关 = 2
set @zt = '单证晚到超过4天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--通关超过5天
set @zt = '通关超过5天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---承运超过4天
set @zt = '承运超过4天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---产生疏港
set @zt = '产生疏港'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---产生滞箱
set @zt = '产生滞箱'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---其他异常
--set @zt = '其他异常'
--set @shux = '票数'
--EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
--insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)
--
--set @shux = '箱量'
--EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
--insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)


-----------------------全部异常清关-----------------------
set @sx = '其中：清关'
set @zt = '总合计数'
set @shux = '票数'
--总合计数 
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--单证晚到超过4天     转关 = 1    清关 = 2
set @zt = '单证晚到超过4天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--通关超过5天
set @zt = '通关超过5天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---承运超过4天
set @zt = '承运超过4天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---产生疏港
set @zt = '产生疏港'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---产生滞箱
set @zt = '产生滞箱'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---其他异常
--set @zt = '其他异常'
--set @shux = '票数'
--EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
--insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)
--
--set @shux = '箱量'
--EXEC 过程查询_报表_进口报关业务月报表_计算全部异常操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
--insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)


-----------------------废纸操作情况-----------------------
set @sx = '废纸操作情况'
set @zt = '总合计数'
set @shux = '票数'
--总合计数 
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--单证晚到超过4天     转关 = 1    清关 = 2
set @zt = '单证晚到超过4天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--通关超过5天
set @zt = '通关超过5天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---承运超过4天
set @zt = '承运超过4天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--产生疏港
set @zt = '产生疏港'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--产生滞箱
set @zt = '产生滞箱'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--其他异常
--set @zt = '其他异常'
--set @shux = '票数'
--EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
--insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)
--
--set @shux = '箱量'
--EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
--insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)



-----------------------废纸转关-----------------------
set @sx = '其中: 转关'
set @zt = '总合计数'
set @shux = '票数'
--总合计数 
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--单证晚到超过4天     转关 = 1    清关 = 2
set @zt = '单证晚到超过4天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--通关超过5天
set @zt = '通关超过5天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---承运超过4天
set @zt = '承运超过4天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--产生疏港
set @zt = '产生疏港'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--产生滞箱
set @zt = '产生滞箱'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--其他异常
--set @zt = '其他异常'
--set @shux = '票数'
--EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
--insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)
--
--set @shux = '箱量'
--EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
--insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)


-----------------------废纸清关-----------------------
set @sx = '其中: 清关'
set @zt = '总合计数'
set @shux = '票数'
--总合计数 
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--单证晚到超过4天     转关 = 1    清关 = 2
set @zt = '单证晚到超过4天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--通关超过5天
set @zt = '通关超过5天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

---承运超过4天
set @zt = '承运超过4天'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--产生疏港
set @zt = '产生疏港'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--产生滞箱
set @zt = '产生滞箱'
set @shux = '票数'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

set @shux = '箱量'
EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

--其他异常
--set @zt = '其他异常'
--set @shux = '票数'
--EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
--insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)
--
--set @shux = '箱量'
--EXEC 过程查询_报表_进口报关业务月报表_计算废纸操作情况 @sx,@zt,@shux,@value output,@日期Ge,@日期Le
--insert into Temp_进口报关业务月报表 values(@sx,@zt,@shux,@value)

exec ('select * from Temp_进口报关业务月报表')
drop table Temp_进口报关业务月报表
end











CREATE proc [dbo].[过程查询_报表_进口报关业务月报表_计算废纸操作情况](@sx varchar(20),@zt varchar(20),@shux varchar(10),@value int output,@日期Ge datetime,@日期Le datetime)
as
begin
--------------------------废纸操作情况---------------------------
	if @sx = '废纸操作情况'
	begin
		if @zt = '总合计数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (单证晚到 > 0 or 通关天数 > 5 or 承运超期 > 0) and 货物类别 like '%废纸%'
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (单证晚到 > 0 or 通关天数 > 5 or 承运超期 > 0) and 货物类别 like '%废纸%'
			end
		end

		if @zt = '单证晚到超过4天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 单证晚到 > 4 and 货物类别 like '%废纸%' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 单证晚到 > 4 and 货物类别 like '%废纸%'
			end
		end

		if @zt = '通关超过5天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 通关天数 > 5 and 货物类别 like '%废纸%' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 通关天数 > 5 and 货物类别 like '%废纸%'
			end
		end

		if @zt = '承运超过4天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 承运超期 > 4 and 货物类别 like '%废纸%' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 承运超期 > 4 and 货物类别 like '%废纸%'
			end
		end

		if @zt = '产生疏港'
		begin
			if @shux = '票数'
			begin 
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where datediff(day,到港时间,开始时间) > 15 and 货物类别 like '%废纸%' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where datediff(day,到港时间,开始时间) > 15 and 货物类别 like '%废纸%'
			end
		end

		if @zt = '产生滞箱'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 滞箱费减免标志 = 'False' and 货物类别 like '%废纸%' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 滞箱费减免标志 = 'False' and 货物类别 like '%废纸%'
			end
		end
	end


---------------------废纸转关--------------------------
	if @sx = '其中: 转关'
	begin
		if @zt = '总合计数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (单证晚到 > 0 or 通关天数 > 5 or 承运超期 > 0) and 通关类别 = 1 and 货物类别 like '%废纸%'
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (单证晚到 > 0 or 通关天数 > 5 or 承运超期 > 0) and 通关类别 = 1 and 货物类别 like '%废纸%'
			end
		end

		if @zt = '单证晚到超过4天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 单证晚到 > 4 and 通关类别 = 1 and 货物类别 like '%废纸%' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 单证晚到 > 4 and 通关类别 = 1 and 货物类别 like '%废纸%'
			end
		end

		if @zt = '通关超过5天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 通关天数 > 5 and 通关类别 = 1 and 货物类别 like '%废纸%' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 通关天数 > 5 and 通关类别 = 1 and 货物类别 like '%废纸%'
			end
		end

		if @zt = '承运超过4天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 承运超期 > 4 and 通关类别 = 1 and 货物类别 like '%废纸%' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 承运超期 > 4 and 通关类别 = 1 and 货物类别 like '%废纸%'
			end
		end

		if @zt = '产生疏港'
		begin
			if @shux = '票数'
			begin 
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where datediff(day,到港时间,开始时间) > 15 and 通关类别 = 1 and 货物类别 like '%废纸%' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where datediff(day,到港时间,开始时间) > 15 and 通关类别 = 1 and 货物类别 like '%废纸%' 
			end
		end

		if @zt = '产生滞箱'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 滞箱费减免标志 = 'False' and 通关类别 = 1 and 货物类别 like '%废纸%' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 滞箱费减免标志 = 'False' and 通关类别 = 1 and 货物类别 like '%废纸%'
			end
		end
	end


---------------------废纸清关--------------------------
	if @sx = '其中: 清关'
	begin
		if @zt = '总合计数'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (单证晚到 > 0 or 通关天数 > 5 or 承运超期 > 0) and 通关类别 = 2 and 货物类别 like '%废纸%'
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where (单证晚到 > 0 or 通关天数 > 5 or 承运超期 > 0) and 通关类别 = 2 and 货物类别 like '%废纸%'
			end
		end

		if @zt = '单证晚到超过4天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 单证晚到 > 4 and 通关类别 = 2 and 货物类别 like '%废纸%' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 单证晚到 > 4 and 通关类别 = 2 and 货物类别 like '%废纸%'
			end
		end

		if @zt = '通关超过5天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 通关天数 > 5 and 通关类别 = 2 and 货物类别 like '%废纸%' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 通关天数 > 5 and 通关类别 = 2 and 货物类别 like '%废纸%'
			end
		end

		if @zt = '承运超过4天'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 承运超期 > 4 and 通关类别 = 2 and 货物类别 like '%废纸%' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 承运超期 > 4 and 通关类别 = 2 and 货物类别 like '%废纸%'
			end
		end

		if @zt = '产生疏港'
		begin
			if @shux = '票数'
			begin 
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where datediff(day,到港时间,开始时间) > 15 and 通关类别 = 2 and 货物类别 like '%废纸%' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where datediff(day,到港时间,开始时间) > 15 and 通关类别 = 2 and 货物类别 like '%废纸%'
			end
		end

		if @zt = '产生滞箱'
		begin
			if @shux = '票数'
			begin
				select @value = count(委托人) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 滞箱费减免标志 = 'False' and 通关类别 = 2 and 货物类别 like '%废纸%' 
			end
			if @shux = '箱量'
			begin
				select @value = sum(箱量) from 函数查询_报表_进口报关业务月报表(@日期Ge,@日期Le) where 滞箱费减免标志 = 'False' and 通关类别 = 2 and 货物类别 like '%废纸%'
			end
		end
	end



end






create proc [dbo].[过程查询_交叉表_对账单_进口其他业务](@对账单 varchar(10))
as
begin

declare @sql varchar(8000)
set @sql = 'select 编号 as 对账单,提单号,箱量,箱型,船名航次,品名,到港时间,委托人,起始日期,关账日期'
select @sql = @sql + ' , max(case 费用项编号 when ''' + 费用项编号 + ''' then 金额 else null end) [' + 费用项 + ']'
from (select distinct 费用项,费用项编号 from 报表_进口其他业务 where 编号 = @对账单) as a 
set @sql = @sql + ' from 报表_进口其他业务 where 编号 ='''+ @对账单 +''' group by 编号,提单号,委托人,品名,船名航次,箱量,箱型,到港时间,起始日期,关账日期 order by 提单号'
exec(@sql)

end
CREATE proc [dbo].[过程更新_进口_滞箱费减免]
as
begin
	exec 过程更新_财务_费用信息附加字段_滞箱费
	exec 过程更新_业务备案_进口票
	update 业务备案_进口票 set 滞箱费减免标志 = 'False'
	update 业务备案_进口票 set 滞箱费减免标志 = 'True'
	where id in 
(select a.费用实体 from 财务_费用 a where a.费用项 = 167
group by a.费用实体)

	update 业务备案_进口票 set 滞箱费减免标志 = 'True'
	where 单证晚到 >= 5 or 异常情况 like '21,' or 预计用箱天数 > 免箱天数
	
update 业务备案_进口票 set 滞箱费警示状态 = (select 滞箱费警示状态 from 函数更新_业务备案_进口票_滞箱费警示状态(a.id))
from 业务备案_进口票 as a where 滞箱费减免标志 = 'True'
end


CREATE VIEW dbo.视图查询_应收应付日记帐_余额1
AS
SELECT     TOP (100) PERCENT 当前行数, 金额, 相关人, 收付标志, 费用项, 类型, 相关号, 增, 减, 结算期限, 源,
                          (SELECT     SUM(金额) AS 余额
                            FROM          dbo.视图查询_应收应付日记帐_行数1 AS t2
                            WHERE      (t1.相关人 = 相关人) AND (t1.收付标志 = 收付标志) AND (t1.类型 = 类型) AND (当前行数 <= t1.当前行数)) AS 余额, 日期, 业务类型
FROM         dbo.视图查询_应收应付日记帐_行数1 AS t1

CREATE VIEW dbo.查询统计_发票日记帐
AS
SELECT     '发票' AS 源, Id, 日期 AS 入账日期, 日期 AS Expr1, 发票账户, 票据号码, 金额 AS 开票增, NULL AS 成本发票减少, 相关人, 开票类别, 对账单, 是否作废, 
                      内容, 备注
FROM         dbo.财务_发票 AS A
UNION ALL
SELECT     '成本发票' AS 源, Id, 入账日期, 日期, 发票账户, NULL AS 票据号码, NULL AS 开票增, 金额 AS 成本发票减, 相关人, NULL AS 开票类别, NULL 
                      AS 对账单, NULL AS Expr1, 内容, 备注
FROM         dbo.财务_成本发票 AS B

CREATE VIEW dbo.视图查询_应收应付日记帐_余额
AS
SELECT     TOP (100) PERCENT 当前行数, 金额, 相关人, 收付标志, 费用项, 相关号, 增, 减, 结算期限, 源,
                          (SELECT     SUM(金额) AS 余额
                            FROM          dbo.视图查询_应收应付日记帐_行数 AS t2
                            WHERE      (t1.相关人 = 相关人) AND (t1.收付标志 = 收付标志) AND (t1.费用项 = 费用项) AND (当前行数 <= t1.当前行数)) AS 余额, 日期, 
                      业务类型
FROM         dbo.视图查询_应收应付日记帐_行数 AS t1

CREATE VIEW [dbo].[视图查询_应收应付日记帐_行数1]
AS
SELECT     ROW_NUMBER() OVER (partition BY 相关人, 收付标志, 类型
ORDER BY 日期) AS 当前行数, 金额, 相关人, 收付标志, 费用项, 类型, 业务类型, 相关号, CASE WHEN 金额 < 0 THEN NULL ELSE 金额 END AS 增, 
CASE WHEN 金额 <= 0 THEN 0 - 金额 ELSE NULL END AS 减, 结算期限, 源, 日期
FROM         dbo.视图查询_应收应付明细
WHERE     源 NOT LIKE '%专款专用%' AND 源 <> '未凭证未对账'



CREATE VIEW [dbo].[报表_滞箱费减免联系单]
AS
SELECT     b.委托人, b.货代自编号, a.单证齐全时间, a.到港时间, b.合同号, b.提单号, b.总重量, b.箱量, d.箱型, a.免箱天数, a.用箱天数, SUM(c.初始滞箱费) AS 滞箱费, 
                      b.对上备注 AS 备注, b.船公司
FROM         dbo.业务备案_进口票 AS a LEFT OUTER JOIN
                      dbo.业务备案_普通票 AS b ON a.Id = b.Id LEFT OUTER JOIN
                      dbo.业务备案_进口箱 AS c ON a.Id = c.票 LEFT OUTER JOIN
                      dbo.业务备案_普通箱 AS d ON d.Id = c.Id
WHERE     (a.滞箱费减免标志 = 'True')
GROUP BY b.委托人, b.货代自编号, a.单证齐全时间, a.到港时间, b.合同号, b.提单号, b.总重量, b.箱量, d.箱型, a.免箱天数, a.用箱天数, c.初始滞箱费, b.对上备注, 
                      b.船公司


CREATE VIEW [dbo].[视图_票费用类别]
AS
SELECT     A.费用类别, A.费用实体 AS 票, B.费用实体类型 AS 业务类型
FROM         dbo.财务_费用 AS A INNER JOIN
                      dbo.财务_费用实体 AS B ON A.费用实体 = B.Id
WHERE     (A.费用信息 IS NULL) AND (A.费用类别 < 40)
GROUP BY A.费用类别, A.费用实体, B.费用实体类型


CREATE VIEW [dbo].[视图_未处理费用信息]
AS
SELECT     费用信息
FROM         dbo.财务_费用
WHERE     (费用信息 IS NOT NULL) AND (对账单 IS NULL) AND (凭证费用明细 IS NULL)
GROUP BY 费用信息


CREATE VIEW [dbo].[视图_应收应付凭证Id]
AS
SELECT     b.Id, b.凭证号
FROM         dbo.财务_凭证费用明细 AS a INNER JOIN
                      dbo.财务_凭证 AS b ON a.凭证 = b.Id
WHERE     (a.费用项 IN ('012', '002', '000', '001'))


CREATE VIEW [dbo].[视图费用类别_业务]
AS
SELECT     费用类别 AS 代码, 大类, 小类, 45 AS 业务类型
FROM         dbo.视图费用项_业务
WHERE     (现有费用实体类型 LIKE '%45,%')
GROUP BY 小类, 费用类别, 大类
UNION ALL
SELECT     费用类别 AS 代码, 大类, 小类, 11 AS 业务类型
FROM         dbo.视图费用项_业务 AS 视图费用项_业务_3
WHERE     (现有费用实体类型 LIKE '%11,%')
GROUP BY 小类, 费用类别, 大类
UNION ALL
SELECT     费用类别 AS 代码, 大类, 小类, 13 AS 业务类型
FROM         dbo.视图费用项_业务 AS 视图费用项_业务_2
WHERE     (现有费用实体类型 LIKE '%13,%')
GROUP BY 小类, 费用类别, 大类
UNION ALL
SELECT     费用类别 AS 代码, 大类, 小类, 15 AS 业务类型
FROM         dbo.视图费用项_业务 AS 视图费用项_业务_1
WHERE     (现有费用实体类型 LIKE '%15,%')
GROUP BY 小类, 费用类别, 大类


CREATE VIEW [dbo].[视图财务_可支付票据]
AS
SELECT     '承兑汇票' AS 源, C.Id, C.票据号码
FROM         dbo.财务_承兑汇票 AS C INNER JOIN
                      dbo.财务_费用实体 AS A ON C.Id = A.Id
WHERE     (C.托收贴现 IS NULL) AND (A.Submitted = 0)
UNION ALL
SELECT     CASE 支票类型 WHEN '1' THEN '现金支票' ELSE '转账支票' END AS 源, Id, 票据号码
FROM         dbo.财务_支票
WHERE     (是否作废 = 0) AND (金额 IS NOT NULL) AND (Submitted = 0)


CREATE VIEW [dbo].[视图查询月报表_非业务及业务常规费用明细]
AS
SELECT     '业务' AS 源, 金额小计 AS 金额, 收付标志, 200 + 费用实体类型 AS 分组, 
                      CASE 费用实体类型 WHEN 11 THEN '进口' WHEN 15 THEN '内贸出港' WHEN 45 THEN '进口其他业务' END AS 小类, 大类, 票时间 AS 入账日期
FROM         dbo.视图查询_业务费用小计 AS A
WHERE     (大类 = '业务常规')
UNION ALL
SELECT     '非业务' AS 源, A.金额, A.收付标志, 100 + A.费用类别 AS 分组, B.小类, B.大类, A.入账日期
FROM         dbo.视图查询_非业务费用明细 AS A INNER JOIN
                      dbo.视图信息_报表项目_月报表1 AS B ON 100 + A.费用类别 = B.代码


CREATE VIEW [dbo].[视图查询_承兑汇票]
AS
SELECT     C.Id, C.票据号码, C.出票银行, C.承兑期限, C.付款人, C.托收贴现, C.备注, C.摘要, C.金额, A.Submitted, C.返回时间
FROM         dbo.财务_承兑汇票 AS C INNER JOIN
                      dbo.财务_费用实体 AS A ON C.Id = A.Id


CREATE VIEW dbo.视图查询_承兑汇票_凭证收付
AS
SELECT     TOP (100) PERCENT B.Id AS 凭证收支明细, B.收付标志, C.票据号码, C.Id AS 承兑汇票, B.Id, B.凭证, A.凭证号, A.摘要 AS 凭证摘要, A.日期, 
                      C.出票银行 AS 所属银行, C.付款人 AS 来源, B.票据号码 AS 票号, C.金额, C.托收贴现, C.承兑期限 AS 到期时间, C.返回时间
FROM         dbo.财务_凭证 AS A INNER JOIN
                      dbo.财务_凭证收支明细 AS B ON A.Id = B.凭证 INNER JOIN
                      dbo.财务_承兑汇票 AS C ON B.票据号码 = C.票据号码 AND B.收付款方式 = 4
WHERE     (A.收支状态 = 1)

CREATE VIEW [dbo].[视图查询_票费用项_费用信息完全]
AS
SELECT     TOP (100) PERCENT c.Id, a.编号, d.Submitted
FROM         dbo.参数备案_费用项 AS a INNER JOIN
                      dbo.信息_费用类别 AS b ON a.收入类别 = b.代码 AND b.大类 = '业务常规' AND a.现有费用实体类型 LIKE '%11,%' CROSS JOIN
                      dbo.业务备案_进口票 AS c LEFT OUTER JOIN
                      dbo.财务_费用信息 AS d ON c.Id = d.票 AND a.编号 = d.费用项


CREATE VIEW [dbo].[视图查询_票费用项_金额]
AS
SELECT     a.费用项, a.对账单 AS 对账单Id, a.收付标志, b.编号 AS 对账单, c.货代自编号, d.Submitted, SUM(ISNULL(a.金额, 0)) AS 金额, d.票, d.委托人承担, 
                      d.车队承担, d.对外付款, d.自己承担, b.关账日期
FROM         dbo.业务备案_普通票 AS c INNER JOIN
                      dbo.财务_费用信息 AS d ON c.Id = d.票 LEFT OUTER JOIN
                      dbo.财务_费用 AS a ON d.Id = a.费用信息 LEFT OUTER JOIN
                      dbo.财务_对账单 AS b ON a.对账单 = b.Id
GROUP BY a.费用项, a.对账单, a.收付标志, b.编号, c.货代自编号, d.Submitted, d.票, d.委托人承担, d.车队承担, d.对外付款, d.自己承担, b.关账日期



CREATE VIEW dbo.视图查询_票汇总_进口
AS
SELECT     A.Id, A.货代自编号, A.委托时间, A.委托人, A.提单号, A.箱量, A.标箱量, A.总重量, A.船名航次, A.船公司, A.合同号, C.转关标志, C.承运人, C.放行时间, 
                      C.到港时间, A.品名, B.常规已确认收款, B.常规已分离收款, B.常规未分离收款, B.额外已确认收款, B.额外已分离收款, B.额外未分离收款, 
                      B.常规已确认付款, B.常规已分离付款, B.常规未分离付款, B.额外已确认付款, B.额外已分离付款, B.额外未分离付款, B.费用实体, 
                      (ISNULL(B.常规已确认收款, 0) + ISNULL(B.常规已分离收款, 0) + ISNULL(B.常规未分离收款, 0) - ISNULL(B.常规已确认付款, 0) 
                      - ISNULL(B.常规已分离付款, 0) - ISNULL(B.常规未分离付款, 0)) / A.箱量 AS 常规单箱毛利, (ISNULL(B.额外已确认收款, 0) + ISNULL(B.额外已分离收款, 
                      0) + ISNULL(B.额外未分离收款, 0) - ISNULL(B.额外已确认付款, 0) - ISNULL(B.额外已分离付款, 0) - ISNULL(B.额外未分离付款, 0)) 
                      / A.箱量 AS 额外单箱毛利, C.承运标志, C.常规已对账, ISNULL(B.常规已确认收款, 0) + ISNULL(B.常规已分离收款, 0) + ISNULL(B.常规未分离收款, 0) 
                      - ISNULL(B.常规已确认付款, 0) - ISNULL(B.常规已分离付款, 0) - ISNULL(B.常规未分离付款, 0) AS 常规票毛利, ISNULL(B.额外已确认收款, 0) 
                      + ISNULL(B.额外已分离收款, 0) + ISNULL(B.额外未分离收款, 0) - ISNULL(B.额外已确认付款, 0) - ISNULL(B.额外已分离付款, 0) 
                      - ISNULL(B.额外未分离付款, 0) AS 额外票毛利, (ISNULL(B.常规已确认收款, 0) + ISNULL(B.常规已分离收款, 0) + ISNULL(B.常规未分离收款, 0) 
                      - ISNULL(B.常规已确认付款, 0) - ISNULL(B.常规已分离付款, 0) - ISNULL(B.常规未分离付款, 0) + ISNULL(B.额外已确认收款, 0) 
                      + ISNULL(B.额外已分离收款, 0) + ISNULL(B.额外未分离收款, 0) - ISNULL(B.额外已确认付款, 0) - ISNULL(B.额外已分离付款, 0) 
                      - ISNULL(B.额外未分离付款, 0)) / A.箱量 AS 单箱毛利, ISNULL(B.常规已确认收款, 0) + ISNULL(B.常规已分离收款, 0) + ISNULL(B.常规未分离收款, 0) 
                      - ISNULL(B.常规已确认付款, 0) - ISNULL(B.常规已分离付款, 0) - ISNULL(B.常规未分离付款, 0) + ISNULL(B.额外已确认收款, 0) 
                      + ISNULL(B.额外已分离收款, 0) + ISNULL(B.额外未分离收款, 0) - ISNULL(B.额外已确认付款, 0) - ISNULL(B.额外已分离付款, 0) 
                      - ISNULL(B.额外未分离付款, 0) AS 票毛利, B.其他开支预估
FROM         dbo.业务备案_普通票 AS A INNER JOIN
                      dbo.业务备案_进口票 AS C ON A.Id = C.Id LEFT OUTER JOIN
                      dbo.查询_业务费用小计_票汇总 AS B ON A.Id = B.费用实体

CREATE VIEW dbo.视图查询_票汇总_内贸出港
AS
SELECT     A.Id, A.货代自编号, A.委托时间, A.委托人, A.提单号, A.箱量, A.标箱量, A.总重量, A.船名航次, A.船公司, A.合同号, C.承运人, C.开航日期, A.代表性箱号, 
                      A.品名, C.目的港, C.进港地, C.到港时间, C.对港承运人, B.费用实体, B.常规已确认收款, B.常规已分离收款, B.常规未分离收款, B.额外已确认收款, 
                      B.额外已分离收款, B.额外未分离收款, B.常规已确认付款, B.常规已分离付款, B.常规未分离付款, B.额外已确认付款, B.额外已分离付款, 
                      B.额外未分离付款, (ISNULL(B.常规已确认收款, 0) + ISNULL(B.常规已分离收款, 0) + ISNULL(B.常规未分离收款, 0) - ISNULL(B.常规已确认付款, 0) 
                      - ISNULL(B.常规已分离付款, 0) - ISNULL(B.常规未分离付款, 0)) / A.箱量 AS 常规单箱毛利, (ISNULL(B.额外已确认收款, 0) + ISNULL(B.额外已分离收款, 
                      0) + ISNULL(B.额外未分离收款, 0) - ISNULL(B.额外已确认付款, 0) - ISNULL(B.额外已分离付款, 0) - ISNULL(B.额外未分离付款, 0)) 
                      / A.箱量 AS 额外单箱毛利, ISNULL(B.常规已确认收款, 0) + ISNULL(B.常规已分离收款, 0) + ISNULL(B.常规未分离收款, 0) - ISNULL(B.常规已确认付款, 
                      0) - ISNULL(B.常规已分离付款, 0) - ISNULL(B.常规未分离付款, 0) AS 常规票毛利, ISNULL(B.额外已确认收款, 0) + ISNULL(B.额外已分离收款, 0) 
                      + ISNULL(B.额外未分离收款, 0) - ISNULL(B.额外已确认付款, 0) - ISNULL(B.额外已分离付款, 0) - ISNULL(B.额外未分离付款, 0) AS 额外票毛利, 
                      (ISNULL(B.常规已确认收款, 0) + ISNULL(B.常规已分离收款, 0) + ISNULL(B.常规未分离收款, 0) - ISNULL(B.常规已确认付款, 0) 
                      - ISNULL(B.常规已分离付款, 0) - ISNULL(B.常规未分离付款, 0) + ISNULL(B.额外已确认收款, 0) + ISNULL(B.额外已分离收款, 0) 
                      + ISNULL(B.额外未分离收款, 0) - ISNULL(B.额外已确认付款, 0) - ISNULL(B.额外已分离付款, 0) - ISNULL(B.额外未分离付款, 0)) / A.箱量 AS 单箱毛利, 
                      ISNULL(B.常规已确认收款, 0) + ISNULL(B.常规已分离收款, 0) + ISNULL(B.常规未分离收款, 0) - ISNULL(B.常规已确认付款, 0) 
                      - ISNULL(B.常规已分离付款, 0) - ISNULL(B.常规未分离付款, 0) + ISNULL(B.额外已确认收款, 0) + ISNULL(B.额外已分离收款, 0) 
                      + ISNULL(B.额外未分离收款, 0) - ISNULL(B.额外已确认付款, 0) - ISNULL(B.额外已分离付款, 0) - ISNULL(B.额外未分离付款, 0) AS 票毛利, 
                      B.其他开支预估, C.常规已对账
FROM         dbo.业务备案_普通票 AS A INNER JOIN
                      dbo.业务备案_内贸出港票 AS C ON A.Id = C.Id LEFT OUTER JOIN
                      dbo.查询_业务费用小计_票汇总 AS B ON A.Id = B.费用实体

CREATE VIEW [dbo].[视图查询_承兑汇票_托收贴现]
AS
SELECT     托收贴现, Id, 票据号码, 出去时间, 经办人, 出去经手人, 返回时间, 返回方式, 入款账户, 返回经手人, 返回金额
FROM         dbo.财务_承兑汇票 AS C
WHERE     (托收贴现 IS NOT NULL)


CREATE VIEW [dbo].[视图查询_进口_常规应收对账单_明细]
AS
SELECT     a.Id, a.常规费用可对账, a.常规费用已对账, b.委托人, b.货代自编号, b.委托时间, b.提单号, b.标箱量, c.放行时间
FROM         (SELECT     Id, CASE WHEN ('true' = ALL
                                                  (SELECT     submitted
                                                    FROM          视图查询_票费用项_费用信息完全
                                                    WHERE      id = z.id)) THEN CAST('true' AS bit) ELSE CAST('false' AS bit) END AS 常规费用可对账, CASE WHEN
                                                  ((SELECT     COUNT(*)
                                                      FROM         财务_费用
                                                      WHERE     费用实体 = z.id AND 收付标志 = '1' AND 对账单 IS NOT NULL)) > 0 THEN CAST('true' AS bit) ELSE CAST('false' AS bit) 
                                              END AS 常规费用已对账
                       FROM          dbo.视图查询_票费用项_费用信息完全 AS z
                       GROUP BY Id) AS a INNER JOIN
                      dbo.业务备案_普通票 AS b ON a.Id = b.Id INNER JOIN
                      dbo.业务备案_进口票 AS c ON a.Id = c.Id

CREATE VIEW dbo.视图查询_对账单_票费用项金额
AS
SELECT     SUM(B.金额) AS 金额, B.对账单, A.编号, B.费用实体, CASE WHEN A.对账单类型 = 2 THEN A.费用项 WHEN A.对账单类型 <> 2 AND (b.费用类别 = 21 OR
                      b.费用类别 = 22) THEN '001' ELSE '000' END AS 常规额外, A.关账日期, A.业务类型, B.费用项, A.对账单类型, A.起始日期, C.Submitted, C.委托人承担, 
                      C.车队承担, C.对外付款, C.自己承担, C.完全标志付
FROM         dbo.财务_费用 AS B LEFT OUTER JOIN
                      dbo.财务_对账单 AS A ON A.Id = B.对账单 LEFT OUTER JOIN
                      dbo.财务_费用信息 AS C ON B.费用信息 = C.Id
WHERE     (B.对账单 IS NOT NULL)
GROUP BY B.对账单, B.费用实体, B.费用项, B.费用类别, A.编号, A.关账日期, A.业务类型, A.费用项, A.对账单类型, A.起始日期, C.Submitted, C.委托人承担, 
                      C.车队承担, C.对外付款, C.自己承担, C.完全标志付

CREATE VIEW [dbo].[视图信息_报表项目_月报表1]
AS
SELECT     100 + 代码 AS 代码, 小类, 大类
FROM         dbo.信息_费用类别
WHERE     (大类 <> '业务常规') AND (大类 <> '业务额外')
UNION ALL
SELECT     200 + 费用实体类型 AS 代码, 业务 AS 小类, '业务常规' AS 大类
FROM         dbo.视图信息_费用实体类型_业务
UNION ALL
SELECT     301 AS 代码, '调节款' AS 小类, '调节款坏账' AS 大类
UNION ALL
SELECT     302 AS 代码, '坏账' AS 小类, '调节款坏账' AS 大类


CREATE VIEW [dbo].[视图查询_固定资产折旧]
AS
SELECT     分类, 购入时间, 简介, 购入金额, ISNULL(卖出时间, DATEADD(YY, 使用年限, 购入时间)) AS 资产归零时间, 月折旧额
FROM         dbo.财务_固定资产 AS A


CREATE VIEW [dbo].[视图查询_借贷]
AS
SELECT     A.日期, A.结算期限, A.业务类型, A.费用项, A.金额, A.相关人, A.收付标志, B.凭证号,
                          (SELECT     dbo.Concatenate(DISTINCT 收付款方式) AS Expr1
                            FROM          dbo.财务_凭证收支明细 AS X
                            WHERE      (凭证 = A.应收应付源)) AS 收付款方式,
                          (SELECT     dbo.Concatenate(DISTINCT 备注) AS Expr1
                            FROM          dbo.财务_凭证费用明细 AS X
                            WHERE      (凭证 = A.应收应付源)) AS 用途, B.备注, CASE 收付标志 WHEN 1 THEN '借出款' ELSE '贷款' END AS 类型
FROM         dbo.财务_应收应付款 AS A INNER JOIN
                      dbo.信息_业务类型 AS C ON A.业务类型 = C.代码 LEFT OUTER JOIN
                      dbo.财务_凭证 AS B ON A.应收应付源 = B.Id
WHERE     (A.费用项 = '002') AND (C.支出类别 = 203)


CREATE VIEW [dbo].[视图信息_费用实体_非业务]
AS
SELECT     A.费用实体类型, A.Id, B.票据号码 AS 相关号, C.非业务
FROM         dbo.财务_费用实体 AS A INNER JOIN
                      dbo.财务_承兑汇票 AS B ON A.Id = B.Id AND A.费用实体类型 = 21 INNER JOIN
                      dbo.视图信息_费用实体类型_非业务 AS C ON A.费用实体类型 = C.费用实体类型
UNION ALL
SELECT     A.费用实体类型, A.Id, B.票据号码 AS 相关号, C.非业务
FROM         dbo.财务_费用实体 AS A INNER JOIN
                      dbo.财务_发票 AS B ON A.Id = B.Id AND A.费用实体类型 = 23 INNER JOIN
                      dbo.视图信息_费用实体类型_非业务 AS C ON A.费用实体类型 = C.费用实体类型
UNION ALL
SELECT     A.费用实体类型, A.Id, A.编号 AS 相关号, C.非业务
FROM         dbo.财务_费用实体 AS A INNER JOIN
                      dbo.视图信息_费用实体类型_非业务 AS C ON A.费用实体类型 = C.费用实体类型
WHERE     (C.非业务 <> '发票') AND (C.非业务 <> '承兑汇票')


CREATE VIEW [dbo].[视图查询_票常规应收最早对账日期]
AS
SELECT     MIN(D.关账日期) AS 最早对账日期, C.费用实体
FROM         dbo.财务_对账单 AS D INNER JOIN
                      dbo.财务_费用 AS C ON D.Id = C.对账单
WHERE     (D.对账单类型 = 2) AND (C.费用类别 <= 13) AND (C.收付标志 = 1) AND (D.Submitted = 1)
GROUP BY C.费用实体


CREATE VIEW [dbo].[视图信息_费用实体_相关号]
AS
SELECT     A.费用实体类型, A.Id, B.票据号码 AS 相关号, C.非业务 AS 类型
FROM         dbo.财务_费用实体 AS A INNER JOIN
                      dbo.财务_承兑汇票 AS B ON A.Id = B.Id AND A.费用实体类型 = 21 INNER JOIN
                      dbo.视图信息_费用实体类型_非业务 AS C ON A.费用实体类型 = C.费用实体类型
UNION ALL
SELECT     A.费用实体类型, A.Id, B.票据号码 AS 相关号, C.非业务
FROM         dbo.财务_费用实体 AS A INNER JOIN
                      dbo.财务_发票 AS B ON A.Id = B.Id AND A.费用实体类型 = 23 INNER JOIN
                      dbo.视图信息_费用实体类型_非业务 AS C ON A.费用实体类型 = C.费用实体类型
UNION ALL
SELECT     A.费用实体类型, A.Id, B.简介 AS 相关号, C.非业务
FROM         dbo.财务_费用实体 AS A INNER JOIN
                      dbo.财务_固定资产 AS B ON A.Id = B.Id AND A.费用实体类型 = 24 INNER JOIN
                      dbo.视图信息_费用实体类型_非业务 AS C ON A.费用实体类型 = C.费用实体类型
UNION ALL
SELECT     A.费用实体类型, A.Id, A.编号 AS 相关号, C.非业务
FROM         dbo.财务_费用实体 AS A INNER JOIN
                      dbo.视图信息_费用实体类型_非业务 AS C ON A.费用实体类型 = C.费用实体类型
WHERE     (C.非业务 <> '发票') AND (C.非业务 <> '承兑汇票') AND (C.非业务 <> '固定资产')
UNION ALL
SELECT     A.费用实体类型, A.Id, CASE A.费用实体类型 WHEN 45 THEN B.提单号 ELSE B.货代自编号 END AS 相关号, C.业务
FROM         dbo.财务_费用实体 AS A INNER JOIN
                      dbo.业务备案_普通票 AS B ON A.Id = B.Id INNER JOIN
                      dbo.视图信息_费用实体类型_业务 AS C ON A.费用实体类型 = C.费用实体类型


CREATE VIEW [dbo].[视图查询_票费用项_费用金额]
AS
SELECT     B.相关人, B.费用实体, B.费用项, B.对账单, SUM(B.金额) AS 金额, A.编号
FROM         dbo.财务_费用 AS B LEFT OUTER JOIN
                      dbo.财务_对账单 AS A ON A.Id = B.对账单
WHERE     (B.收付标志 = 1) AND (B.费用类型 = 1)
GROUP BY B.相关人, B.费用实体, B.费用项, B.对账单, A.编号



CREATE VIEW [dbo].[视图信息_代表性箱号]
AS
SELECT     dbo.Concatenate(箱号) AS 代表性箱号, 票
FROM         dbo.视图信息_箱号 AS A
WHERE     (箱号 IN
                          (SELECT     TOP (2) 箱号
                            FROM          dbo.视图信息_箱号 AS B
                            WHERE      (A.票 = 票)))
GROUP BY 票


CREATE VIEW [dbo].[视图信息_费用实体_业务]
AS
SELECT     CASE F.费用实体类型 WHEN 11 THEN G.放行时间 WHEN 15 THEN H.开航日期 WHEN 45 THEN I.到港时间 END AS 票时间, F.业务, B.货代自编号, 
                      B.提单号, B.报检号, B.报关单号, B.合同号, E.Id, F.费用实体类型, E.编号, B.箱量, B.船名航次, B.船公司, B.允许应收对账
FROM         dbo.财务_费用实体 AS E INNER JOIN
                      dbo.视图信息_费用实体类型_业务 AS F ON E.费用实体类型 = F.费用实体类型 LEFT OUTER JOIN
                      dbo.业务备案_普通票 AS B ON E.Id = B.Id LEFT OUTER JOIN
                      dbo.业务备案_进口票 AS G ON G.Id = B.Id LEFT OUTER JOIN
                      dbo.业务备案_内贸出港票 AS H ON H.Id = B.Id LEFT OUTER JOIN
                      dbo.业务备案_进口其他业务票 AS I ON I.Id = B.Id


CREATE VIEW [dbo].[视图查询_票费用项_费用信息]
AS
SELECT     a.票, a.费用项, a.Submitted, b.货代自编号, a.委托人承担, a.车队承担, a.对外付款, a.自己承担
FROM         dbo.财务_费用信息 AS a INNER JOIN
                      dbo.业务备案_普通票 AS b ON a.票 = b.Id



CREATE VIEW [dbo].[视图查询_应收应付日记帐_行数]
AS
SELECT     ROW_NUMBER() OVER (partition BY 相关人, 收付标志, 费用项
ORDER BY 日期) AS 当前行数, 金额, 相关人, 收付标志, 费用项, 业务类型, 相关号, CASE WHEN 金额 < 0 THEN NULL ELSE 金额 END AS 增, 
CASE WHEN 金额 <= 0 THEN 0 - 金额 ELSE NULL END AS 减, 结算期限, 源, 日期
FROM         dbo.视图查询_应收应付明细
WHERE     源 NOT LIKE '%专款专用%' AND 源 <> '未凭证未对账'




CREATE VIEW [dbo].[报表_凭证收支明细]
AS
SELECT     a.收付款方式, a.承兑期限, a.票据号码, a.银行账户, (CASE WHEN 收付标志 = 1 THEN 金额 END) AS 收款金额, 
                      (CASE WHEN 收付标志 = 2 THEN 金额 END) AS 付款金额, a.付款人, a.出票银行, b.凭证号
FROM         dbo.财务_凭证收支明细 AS a LEFT OUTER JOIN
                      dbo.财务_凭证 AS b ON a.凭证 = b.Id


CREATE VIEW [dbo].[视图查询_应收应付明细]
AS
SELECT     SUM(A.金额) AS 金额, A.相关人, A.收付标志, A.费用项, A.业务类型, A.日期, B.编号 AS 相关号, A.结算期限, B.源, 
                      CASE A.费用项 WHEN '002' THEN '其他' ELSE '业务' END AS 类型
FROM         dbo.财务_应收应付款 AS A INNER JOIN
                      dbo.视图信息_应收应付源 AS B ON A.应收应付源 = B.Id
GROUP BY A.相关人, A.收付标志, A.费用项, A.业务类型, A.日期, B.编号, A.结算期限, B.源
UNION ALL
SELECT     SUM(A.金额) AS 金额, A.相关人, A.收付标志, A.费用项, A.业务类型, B.日期, B.凭证号 AS 相关号, NULL AS 结算期限, '非业务专款专用' AS '源', 
                      '其他' AS 类型
FROM         dbo.财务_凭证费用明细 AS A INNER JOIN
                      dbo.财务_凭证 AS B ON A.凭证 = B.Id AND B.审核状态 = 1 INNER JOIN
                      dbo.视图费用项_非业务 AS C ON A.费用项 = C.编号
GROUP BY A.相关人, A.收付标志, B.日期, B.凭证号, A.费用项, A.业务类型
UNION ALL
SELECT     SUM(A.金额) AS 金额, A.相关人, A.收付标志, A.费用项, A.业务类型, B.日期, B.凭证号 AS 相关号, NULL AS 结算期限, '业务专款专用' AS '源', 
                      '业务' AS 类型
FROM         dbo.财务_凭证费用明细 AS A INNER JOIN
                      dbo.财务_凭证 AS B ON A.凭证 = B.Id AND B.审核状态 = 1 INNER JOIN
                      dbo.视图费用项_业务 AS C ON A.费用项 = C.编号
GROUP BY A.相关人, A.收付标志, B.日期, B.凭证号, A.费用项, A.业务类型
UNION ALL
SELECT     SUM(金额) AS 金额, 相关人, 收付标志, NULL AS 结算期限, NULL AS Expr1, NULL AS Expr2, NULL AS 相关号, NULL AS Expr3, '未凭证未对账' AS '源', 
                      CASE WHEN a.费用类型 = 1 THEN '业务' ELSE '其他' END AS 类型
FROM         dbo.财务_费用 AS A
WHERE     (凭证费用明细 IS NULL) AND (对账单 IS NULL) AND (金额 IS NOT NULL) AND (相关人 IS NOT NULL)
GROUP BY 相关人, 收付标志, CASE WHEN a.费用类型 = 1 THEN '业务' ELSE '其他' END



CREATE VIEW [dbo].[报表_凭证费用明细]
AS
SELECT     a.相关人 AS 相关人编号, a.金额, a.费用项 AS 费用项编号, b.凭证号, b.日期, (CASE WHEN 收付标志 = 1 THEN a.金额 END) AS 收款金额, 
                      (CASE WHEN 收付标志 = 2 THEN a.金额 END) AS 付款金额, a.业务类型, a.凭证费用类别, a.备注, a.收付标志
FROM         dbo.财务_凭证费用明细 AS a INNER JOIN
                      dbo.财务_凭证 AS b ON a.凭证 = b.Id


CREATE VIEW [dbo].[报表_进口报关业务明细]
AS
SELECT     b.委托人, b.货代自编号 AS 自编号, b.提单号, b.货物类别, b.箱量, a.承运标志, a.转关标志 AS 通关类别, a.单证齐全时间, b.委托时间, a.到港时间, 
                      a.单证晚到, (CASE WHEN 到港时间 IS NULL OR
                      单证齐全时间 IS NULL THEN NULL WHEN 到港时间 >= 单证齐全时间 THEN 到港时间 WHEN 到港时间 < 单证齐全时间 THEN 单证齐全时间 END) 
                      AS 报关时间, a.放行时间, a.通关天数, (CASE WHEN 承运标志 = 0 THEN NULL ELSE a.放行时间 END) AS 开始时间, a.结束时间, a.承运超期, 
                      b.内部备注 AS 备注, a.滞箱费减免标志, b.Id
FROM         dbo.业务备案_进口票 AS a INNER JOIN
                      dbo.业务备案_普通票 AS b ON a.Id = b.Id


CREATE VIEW [dbo].[视图查询_业务费用小计]
AS
SELECT     F.费用项, F.收付标志, F.相关人, F.费用实体, SUM(F.金额) AS 金额小计, SUM(CASE WHEN 凭证费用明细 IS NOT NULL OR
                      G.Submitted = 1 THEN F.金额 ELSE NULL END) AS 已确认金额, SUM(CASE WHEN 凭证费用明细 IS NULL AND (G.Submitted = 0 OR
                      B.Submitted = 1) THEN F.金额 ELSE NULL END) AS 已分离金额, SUM(CASE WHEN 凭证费用明细 IS NULL AND 对账单 IS NULL AND 
                      B.Submitted = 0 THEN F.金额 ELSE NULL END) AS 未分离金额, A.票时间, A.费用实体类型, F.费用类别, D.大类, B.Submitted AS 完成标志, 
                      A.允许应收对账
FROM         dbo.财务_费用 AS F INNER JOIN
                      dbo.视图信息_费用实体_业务 AS A ON F.费用实体 = A.Id LEFT OUTER JOIN
                      dbo.财务_对账单 AS G ON F.对账单 = G.Id LEFT OUTER JOIN
                      dbo.视图费用类别_业务 AS D ON F.费用类别 = D.代码 AND D.业务类型 = A.费用实体类型 LEFT OUTER JOIN
                      dbo.财务_费用信息 AS B ON F.费用信息 = B.Id
WHERE     (F.费用类型 = 1) OR
                      (F.费用类型 = 3)
GROUP BY F.费用项, F.收付标志, F.相关人, F.费用实体, A.票时间, A.费用实体类型, F.费用类别, D.大类, B.Submitted, A.允许应收对账


CREATE VIEW [dbo].[报表_工资单]
AS
SELECT     b.编号 AS 工资单号, a.员工, a.简介, a.备注, ISNULL(a.基本工资, 0) AS 基本工资, ISNULL(a.餐费, 0) AS 餐费, ISNULL(a.通讯费, 0) AS 通讯费, 
                      ISNULL(a.福利, 0) AS 福利, ISNULL(a.补助, 0) AS 补助, ISNULL(a.违纪扣款, 0) AS 违纪扣款, ISNULL(a.养老扣款, 0) AS 养老扣款, ISNULL(a.医疗扣款, 0) 
                      AS 医疗扣款, ISNULL(a.失业扣款, 0) AS 失业扣款, ISNULL(a.其他扣款, 0) AS 其他扣款
FROM         dbo.财务_工资单 AS a INNER JOIN
                      dbo.财务_费用实体 AS b ON a.Id = b.Id



CREATE VIEW [dbo].[视图查询_业务费用小计_票汇总]
AS
SELECT     票时间, 费用实体, SUM(CASE WHEN 收付标志 = 1 AND 大类 = '业务常规' THEN (已确认金额) ELSE NULL END) AS 常规已确认收款, 
                      SUM(CASE WHEN 收付标志 = 1 AND 大类 = '业务常规' THEN (已分离金额) ELSE NULL END) AS 常规已分离收款, SUM(CASE WHEN 收付标志 = 1 AND 
                      大类 = '业务常规' THEN (未分离金额) ELSE NULL END) AS 常规未分离收款, SUM(CASE WHEN 收付标志 = 1 AND 大类 = '业务额外' THEN (已确认金额) 
                      ELSE NULL END) AS 额外已确认收款, SUM(CASE WHEN 收付标志 = 1 AND 大类 = '业务额外' THEN (已分离金额) ELSE NULL END) 
                      AS 额外已分离收款, SUM(CASE WHEN 收付标志 = 1 AND 大类 = '业务额外' THEN (未分离金额) ELSE NULL END) AS 额外未分离收款, 
                      SUM(CASE WHEN 收付标志 = 2 AND 大类 = '业务常规' THEN (已确认金额) ELSE NULL END) AS 常规已确认付款, SUM(CASE WHEN 收付标志 = 2 AND 
                      大类 = '业务常规' THEN (已分离金额) ELSE NULL END) AS 常规已分离付款, SUM(CASE WHEN 收付标志 = 2 AND 大类 = '业务常规' THEN (未分离金额) 
                      ELSE NULL END) AS 常规未分离付款, SUM(CASE WHEN 收付标志 = 2 AND 大类 = '业务额外' THEN (已确认金额) ELSE NULL END) 
                      AS 额外已确认付款, SUM(CASE WHEN 收付标志 = 2 AND 大类 = '业务额外' THEN (已分离金额) ELSE NULL END) AS 额外已分离付款, 
                      SUM(CASE WHEN 收付标志 = 2 AND 大类 = '业务额外' THEN (未分离金额) ELSE NULL END) AS 额外未分离付款
FROM         dbo.视图查询_业务费用小计 AS A
GROUP BY 费用实体, 票时间


CREATE VIEW dbo.视图查询_业务费用明细_费用信息
AS
SELECT     F.Id, F.费用项, F.收付标志, F.相关人, F.费用实体, F.凭证费用明细, F.对账单, F.金额, CASE F.收付标志 WHEN 1 THEN F.金额 ELSE NULL 
                      END AS 收款金额, CASE F.收付标志 WHEN 2 THEN F.金额 ELSE NULL END AS 付款金额, 
                      CASE WHEN d .大类 = '业务常规' THEN C.最早对账日期 WHEN d .大类 = '业务额外' AND F.对账单 IS NOT NULL 
                      THEN G.关账日期 WHEN d .大类 = '业务额外' AND F.对账单 IS NULL AND F.凭证费用明细 IS NOT NULL THEN H.日期 ELSE NULL END AS 入账日期, 
                      A.业务, A.费用实体类型, A.货代自编号, A.提单号, A.报检号, A.报关单号, A.合同号, D.大类, F.费用类别, F.箱, G.编号, G.Submitted, A.箱量, A.船名航次, 
                      A.船公司, F.费用信息, A.票时间, B.Submitted AS 完全标志收, B.完全标志付, B.凭证号, G.编号 AS 对账单号, E.备注, H.相关人 AS 经手人, 
                      B.付款对账凭证状态, B.收款对账凭证状态, B.填全状态, B.警示标志, B.委托人承担, B.车队承担, B.对外付款, B.自己承担, B.Created, B.创建天数, 
                      B.业务类型
FROM         dbo.财务_费用 AS F LEFT OUTER JOIN
                      dbo.财务_凭证费用明细 AS E ON F.凭证费用明细 = E.Id INNER JOIN
                      dbo.视图信息_费用实体_业务 AS A ON F.费用实体 = A.Id LEFT OUTER JOIN
                      dbo.财务_凭证 AS H ON E.凭证 = H.Id LEFT OUTER JOIN
                      dbo.财务_对账单 AS G ON F.对账单 = G.Id LEFT OUTER JOIN
                      dbo.视图费用类别_业务 AS D ON F.费用类别 = D.代码 AND D.业务类型 = A.费用实体类型 LEFT OUTER JOIN
                      dbo.视图查询_票常规应收最早对账日期 AS C ON F.费用实体 = C.费用实体 LEFT OUTER JOIN
                      dbo.财务_费用信息 AS B ON F.费用信息 = B.Id
WHERE     (F.费用类型 = 1) OR
                      (F.费用类型 = 3)

CREATE VIEW dbo.视图查询_资金票据_应收应付当前
AS
SELECT     a.相关人, a.收付标志, a.类型, SUM(a.金额) AS 拟收拟付, SUM(CASE WHEN 源 = '未凭证未对账' THEN 0 ELSE 金额 END) AS 应收应付, 
                      SUM(CASE WHEN 结算期限 <= getdate() THEN 金额 ELSE 0 END) AS 结算到期, ISNULL((CASE WHEN 收付标志 = 1 THEN dateadd(dd, 
                      - b.常规费用结账时间, getdate()) ELSE NULL END), DATEADD(dd, - 90, GETDATE())) AS 结算期限
FROM         dbo.视图查询_应收应付明细 AS a LEFT OUTER JOIN
                      dbo.参数备案_委托人合同 AS b ON a.相关人 = b.委托人
WHERE     (a.源 NOT LIKE '%专款专用%')
GROUP BY a.相关人, a.收付标志, a.类型, b.常规费用结账时间

CREATE VIEW [dbo].[视图查询_业务费用明细]
AS
SELECT     F.Id, F.费用项, F.收付标志, F.相关人, F.费用实体, F.凭证费用明细, F.对账单, F.金额, CASE F.收付标志 WHEN 1 THEN F.金额 ELSE NULL END AS 收款金额, 
                      CASE F.收付标志 WHEN 2 THEN F.金额 ELSE NULL END AS 付款金额, 
                      CASE WHEN d .大类 = '业务常规' THEN C.最早对账日期 WHEN d .大类 = '业务额外' AND F.对账单 IS NOT NULL 
                      THEN G.关账日期 WHEN d .大类 = '业务额外' AND F.对账单 IS NULL AND F.凭证费用明细 IS NOT NULL THEN H.日期 ELSE NULL END AS 入账日期, 
                      A.业务, A.费用实体类型, A.货代自编号, A.提单号, A.报检号, A.报关单号, A.合同号, D.大类, F.费用类别, F.箱, G.编号 AS 对账单号, G.Submitted AS 对账单提交, 
                      A.箱量, A.船名航次, A.船公司, F.费用信息, A.票时间, B.Submitted AS 完全标志, H.凭证号, F.备注, E.备注 AS 凭证费用明细备注, H.相关人 AS 经手人, 
                      F.Created
FROM         dbo.财务_费用 AS F LEFT OUTER JOIN
                      dbo.财务_凭证费用明细 AS E ON F.凭证费用明细 = E.Id INNER JOIN
                      dbo.视图信息_费用实体_业务 AS A ON F.费用实体 = A.Id LEFT OUTER JOIN
                      dbo.财务_凭证 AS H ON E.凭证 = H.Id LEFT OUTER JOIN
                      dbo.财务_对账单 AS G ON F.对账单 = G.Id LEFT OUTER JOIN
                      dbo.视图费用类别_业务 AS D ON F.费用类别 = D.代码 AND D.业务类型 = A.费用实体类型 LEFT OUTER JOIN
                      dbo.视图查询_票常规应收最早对账日期 AS C ON F.费用实体 = C.费用实体 LEFT OUTER JOIN
                      dbo.财务_费用信息 AS B ON F.费用信息 = B.Id
WHERE     (F.费用类型 = 1) OR
                      (F.费用类型 = 3)


CREATE VIEW [dbo].[视图财务过程_进口票_额外费用_委托人]
AS
SELECT     b.Id, SUM(CASE WHEN a.费用项 = 169 THEN (CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - 金额 END) ELSE 0 END) AS 额外其他费, 
                      SUM(CASE WHEN a.费用项 = 167 THEN (CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - 金额 END) ELSE 0 END) AS 滞箱费, 
                      SUM(CASE WHEN a.费用项 = 165 THEN (CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - 金额 END) ELSE 0 END) AS 额外修洗箱费, 
                      SUM(CASE WHEN a.费用项 = 164 THEN (CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - 金额 END) ELSE 0 END) AS 额外查验费, 
                      SUM(CASE WHEN a.费用项 = 163 THEN (CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - 金额 END) ELSE 0 END) AS 倒箱二次开箱费, 
                      SUM(CASE WHEN a.费用项 = 162 THEN (CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - 金额 END) ELSE 0 END) AS 疏港费, 
                      SUM(CASE WHEN a.费用项 = 161 THEN (CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - 金额 END) ELSE 0 END) AS 堆存费, 
                      SUM(CASE WHEN a.费用项 = 170 THEN (CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - 金额 END) ELSE 0 END) AS 额外指运地其他费, 
                      SUM(CASE WHEN a.费用项 = 242 THEN (CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - 金额 END) ELSE 0 END) AS 改舱单费, 
                      SUM(CASE WHEN a.费用项 = 241 THEN (CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - 金额 END) ELSE 0 END) AS 滞报金, 
                      SUM(CASE WHEN a.费用类别 IN (21, 22) THEN (CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - 金额 END) ELSE 0 END) AS 小计
FROM         dbo.业务备案_普通票 AS c INNER JOIN
                      dbo.业务备案_进口票 AS b ON c.Id = b.Id LEFT OUTER JOIN
                      dbo.财务_费用 AS a ON b.Id = a.费用实体 AND c.委托人 = a.相关人
GROUP BY b.Id


CREATE VIEW dbo.报表_进口其他业务
AS
SELECT     a.金额, a.对账单, a.编号, a.费用实体, a.业务类型, a.费用项 AS 费用项编号, b.提单号, b.箱量, b.船名航次, b.到港时间, b.货物类别 AS 品名, 
                      b.票箱型 AS 箱型, c.名称 AS 费用项, b.委托人, a.起始日期, a.关账日期
FROM         dbo.视图查询_对账单_票费用项金额 AS a INNER JOIN
                      dbo.视图查询_票信息_进口其他业务 AS b ON a.费用实体 = b.Id INNER JOIN
                      dbo.参数备案_费用项 AS c ON a.费用项 = c.编号

CREATE VIEW [dbo].[视图查询_报表值_票箱费用项]
AS
SELECT     收付标志, 费用项, 费用实体, 箱, SUM(CASE WHEN 源 = '理论值' THEN 金额 ELSE 0 END) AS 理论值, 
                      SUM(CASE WHEN 源 = '实际值' THEN 金额 ELSE 0 END) AS 实际值
FROM         dbo.视图查询_报表值明细_票箱费用项
GROUP BY 收付标志, 费用项, 费用实体, 箱



CREATE VIEW [dbo].[视图查询_资金日记帐]
AS
SELECT     '凭证' AS 源, Id, 凭证类别 AS 收付标志, 日期, 币制, 数额 AS 金额, 摘要, Id AS 信息主表Id, 凭证号 AS 相关号码
FROM         dbo.财务_凭证 AS P
WHERE     (收支状态 = 1) AND (Submitted = 1)
UNION
SELECT     '换汇出款' AS 源, Id, 2 AS 收付标志, 日期, 出款币制 AS 币制, 出款数额 AS 金额, 摘要, Id AS 信息主表Id, 编号 AS 相关号码
FROM         dbo.财务_换汇 AS 换汇出
UNION ALL
SELECT     '换汇入款' AS 源, Id, 1 AS 收付标志, 日期, 入款币制 AS 币制, 入款数额 AS 金额, 摘要, Id AS 信息主表Id, 编号 AS 相关号码
FROM         dbo.财务_换汇 AS 换汇入


CREATE VIEW [dbo].[视图查询_资金流水明细]
AS
SELECT     '凭证' AS 源, P.Id, B.收付标志, P.日期, P.币制, B.金额, P.摘要, P.Id AS 信息主表Id, P.凭证号 AS 相关号码, B.收付款方式,
                          (SELECT     dbo.Concatenate(DISTINCT 相关人) AS Expr1
                            FROM          dbo.财务_凭证费用明细 AS X
                            WHERE      (凭证 = P.Id)) AS 相关人,
                          (SELECT     dbo.Concatenate(DISTINCT 费用项) AS Expr1
                            FROM          dbo.财务_凭证费用明细 AS X
                            WHERE      (凭证 = P.Id)) AS 费用项, P.备注
FROM         dbo.财务_凭证 AS P INNER JOIN
                      dbo.财务_凭证收支明细 AS B ON P.Id = B.凭证
WHERE     (P.收支状态 = 1) AND (P.Submitted = 1)
UNION
SELECT     '换汇出款' AS 源, Id, 2 AS 收付标志, 日期, 出款币制 AS 币制, 出款数额 AS 金额, 摘要, Id AS 信息主表Id, 编号 AS 相关号码, 
                      CASE WHEN 出款账户 IS NULL THEN 1 ELSE 6 END AS 收付款方式, 经办人 AS 相关人, NULL AS 费用项, 备注
FROM         dbo.财务_换汇 AS 换汇出
UNION ALL
SELECT     '换汇入款' AS 源, Id, 1 AS 收付标志, 日期, 入款币制 AS 币制, 入款数额 AS 金额, 摘要, Id AS 信息主表Id, 编号 AS 相关号码, 
                      CASE WHEN 入款账户 IS NULL THEN 1 ELSE 6 END AS 收付款方式, 经办人 AS 相关人, NULL AS 费用项, 备注
FROM         dbo.财务_换汇 AS 换汇入


CREATE VIEW [dbo].[视图查询_支票_过程在途]
AS
SELECT     '过程在途' AS 源, C.Id, C.买入时间, C.领用时间, C.经办人, C.领用方式, C.票据号码
FROM         dbo.财务_支票 AS C INNER JOIN
                      dbo.参数备案_银行账户 AS Y ON C.银行账户 = Y.Id


CREATE VIEW [dbo].[视图查询_银行日记帐]
AS
SELECT     CASE 支票类型 WHEN 1 THEN '现金支票取' ELSE '转账支票取' END AS 源, Id, 日期, 2 AS 存取标志, 银行账户, 金额, 摘要, Id AS 信息主表Id, 
                      票据号码 AS 相关号码
FROM         dbo.财务_支票 AS 支票取
WHERE     (Submitted = 1)
UNION ALL
SELECT     '转账支票存' AS 源, Id, 日期, 1 AS Expr1, 入款账户, 金额, 摘要, Id AS 信息主表Id, 票据号码 AS 相关号码
FROM         dbo.财务_支票 AS 转账支票存
WHERE     (支票类型 = 2) AND (支付凭证号 IS NULL) AND (Submitted = 1)
UNION ALL
SELECT     '银行存取' AS 源, Id, 日期, 存取标志, 银行账户, 金额, 备注, Id AS 信息主表Id, 编号 AS 相关号码
FROM         dbo.财务_银行存取款
UNION ALL
SELECT     '换汇取' AS 源, Id, 日期, 2 AS 存取标志, 出款账户 AS 银行账户, 出款数额 AS 金额, 摘要, Id AS 信息主表Id, 编号 AS 相关号码
FROM         dbo.财务_换汇 AS 换汇取
WHERE     (出款账户 IS NOT NULL)
UNION ALL
SELECT     '换汇存' AS 源, Id, 日期, 1 AS 存取标志, 入款账户 AS 银行账户, 入款数额 AS 金额, 摘要, Id AS 信息主表Id, 编号 AS 相关号码
FROM         dbo.财务_换汇 AS 换汇存
WHERE     (入款账户 IS NOT NULL)
UNION ALL
SELECT     '凭证银行收付' AS 源, X.Id, P.日期, X.收付标志 AS 存取标志, X.银行账户, X.金额, P.摘要, P.Id AS 信息主表Id, P.凭证号 AS 相关号码
FROM         dbo.财务_凭证收支明细 AS X INNER JOIN
                      dbo.财务_凭证 AS P ON X.凭证 = P.Id
WHERE     (X.收付款方式 = 5) OR
                      (X.收付款方式 = 6) OR
                      (X.收付款方式 = 7)


CREATE VIEW [dbo].[视图查询_银行日记帐_行数]
AS
SELECT     p2.金额, p2.存取标志, p2.ID, p2.银行账户, p2.源, P2.日期, P2.相关号码, P2.摘要, ROW_NUMBER() OVER (PARTITION BY p2.银行账户
ORDER BY p2.日期, p2.ID) AS 当前行数
FROM         视图查询_银行日记帐 AS p2


CREATE VIEW [dbo].[视图对账单金额]
AS
SELECT     SUM(金额) AS 金额, 对账单
FROM         dbo.财务_费用
WHERE     (对账单 IS NOT NULL)
GROUP BY 对账单


CREATE VIEW [dbo].[报表_处理坏账损失明细]
AS
SELECT     相关人, 金额, 入账日期, 备注, 凭证号, 对账单号
FROM         dbo.视图查询_非业务费用明细
WHERE     (费用项 = '352')




CREATE VIEW [dbo].[视图查询_资金日记帐_余额]
AS
SELECT     ID, 币制, 源, 当前行数, 日期, 相关号码, 摘要, CASE t2.收付标志 WHEN 1 THEN t2.金额 ELSE NULL END AS 借, 
                      CASE t2.收付标志 WHEN 2 THEN t2.金额 ELSE NULL END AS 贷,
                          (SELECT     SUM(CASE WHEN t1.收付标志 = 1 THEN t1.金额 ELSE 0 - t1.金额 END) AS 目前为止合计
                            FROM          dbo.查询_资金日记帐_行数 AS t1
                            WHERE      (当前行数 <= t2.当前行数) AND (币制 = t2.币制)) AS 目前为止合计
FROM         dbo.查询_资金日记帐_行数 AS t2


CREATE VIEW [dbo].[报表_财务费用开支明细]
AS
SELECT     Id, 费用项, 收付标志, 相关人, 费用实体, 凭证费用明细, 金额, 收款金额, 付款金额, 费用类别, 费用实体类型, 入账日期 AS 日期, 相关号, 备注, 凭证Id, 
                      小类, 大类, 凭证号, 经手人
FROM         dbo.视图查询_非业务费用明细
WHERE     (大类 = '财务费用') AND (收付标志 = 2)



CREATE VIEW [dbo].[视图查询_资金日记帐_行数]
AS
SELECT     p2.金额, p2.收付标志, p2.ID, p2.币制, p2.源, P2.日期, P2.相关号码, P2.摘要, ROW_NUMBER() OVER (PARTITION BY p2.币制
ORDER BY p2.日期, p2.ID) AS 当前行数
FROM         视图查询_资金日记帐 AS p2


CREATE VIEW [dbo].[视图查询_银行日记帐_余额]
AS
SELECT     t2.ID, t2.银行账户, Y.币制, t2.源, t2.当前行数, t2.日期, t2.相关号码, t2.摘要, CASE t2.存取标志 WHEN 1 THEN t2.金额 ELSE NULL END AS 借, 
                      CASE t2.存取标志 WHEN 2 THEN t2.金额 ELSE NULL END AS 贷,
                          (SELECT     SUM(CASE WHEN t1.存取标志 = 1 THEN t1.金额 ELSE 0 - t1.金额 END) AS 目前为止合计
                            FROM          dbo.查询_银行日记帐_行数 AS t1
                            WHERE      (当前行数 <= t2.当前行数) AND (银行账户 = t2.银行账户)) AS 目前为止合计
FROM         dbo.查询_银行日记帐_行数 AS t2 INNER JOIN
                      dbo.参数备案_银行账户 AS Y ON t2.银行账户 = Y.Id


CREATE VIEW [dbo].[视图查询_支票]
AS
SELECT     '公司支票' AS 源, C.Id, C.支票类型, C.票据号码, C.银行账户, C.金额, Y.币制, C.备注, C.摘要, C.日期, C.支付凭证号 AS 凭证号, C.是否作废, 
                      C.Submitted
FROM         dbo.财务_支票 AS C INNER JOIN
                      dbo.参数备案_银行账户 AS Y ON C.银行账户 = Y.Id
UNION ALL
SELECT     '外部支票' AS 源, B.Id, CASE B.收付款方式 WHEN 2 THEN '1' WHEN 3 THEN '2' END AS 支票类型, B.票据号码, NULL AS 银行账户, B.金额, 
                      A.币制, NULL AS 备注, NULL AS 摘要, A.日期, A.凭证号, A.是否作废, A.Submitted
FROM         dbo.财务_凭证 AS A INNER JOIN
                      dbo.财务_凭证收支明细 AS B ON A.Id = B.凭证
WHERE     (B.收付款方式 = 2 OR
                      B.收付款方式 = 3) AND (B.收付标志 = 1) AND (A.是否作废 = 0) AND (A.Submitted = 1)


CREATE VIEW [dbo].[视图查询_支票_过程结束]
AS
SELECT     '支出' AS 源, C.Id, NULL AS 入款账户, A.摘要 AS 凭证摘要, B.凭证, A.凭证号, C.日期, C.票据号码
FROM         dbo.财务_凭证 AS A INNER JOIN
                      dbo.财务_凭证收支明细 AS B ON A.Id = B.凭证 INNER JOIN
                      dbo.财务_支票 AS C ON B.票据号码 = C.票据号码
UNION ALL
SELECT     '提现' AS 源, C.Id, NULL AS 入款账户, NULL AS 凭证摘要, NULL AS 凭证, NULL AS 凭证号, C.日期, C.票据号码
FROM         dbo.财务_支票 AS C INNER JOIN
                      dbo.参数备案_银行账户 AS Y ON C.银行账户 = Y.Id
WHERE     (C.支票类型 = 1) AND (C.支付凭证号 IS NULL) AND (C.Submitted = 1)
UNION ALL
SELECT     '转账' AS 源, C.Id, C.入款账户, NULL AS 凭证摘要, NULL AS 凭证, NULL AS 凭证号, C.日期, C.票据号码
FROM         dbo.财务_支票 AS C INNER JOIN
                      dbo.参数备案_银行账户 AS Y ON C.银行账户 = Y.Id
WHERE     (C.支票类型 = 2) AND (C.支付凭证号 IS NULL) AND (C.Submitted = 1)
UNION ALL
SELECT     '作废' AS 源, C.Id, NULL AS 入款账户, NULL AS 凭证摘要, NULL AS 凭证, NULL AS 凭证号, C.日期, C.票据号码
FROM         dbo.财务_支票 AS C INNER JOIN
                      dbo.参数备案_银行账户 AS Y ON C.银行账户 = Y.Id
WHERE     (C.是否作废 = 1)
UNION ALL
SELECT     '收入' AS 源, B.Id, B.银行账户 AS 入款账户, A.摘要 AS 凭证摘要, B.凭证, A.凭证号, A.日期, B.票据号码
FROM         dbo.财务_凭证 AS A INNER JOIN
                      dbo.财务_凭证收支明细 AS B ON A.Id = B.凭证
WHERE     (B.收付款方式 = 2 OR
                      B.收付款方式 = 3) AND (B.收付标志 = 1)


CREATE VIEW [dbo].[视图信息_箱号]
AS
SELECT     B.Id, A.箱号, B.票, B.业务类型, A.箱型, A.封志号, (CASE WHEN B.ID = C.ID THEN c.还箱时间 WHEN C.ID = D .ID THEN d .还箱时间 ELSE NULL END) 
                      AS 还箱时间
FROM         dbo.业务备案_普通箱 AS A INNER JOIN
                      dbo.视图信息_箱票 AS B ON A.Id = B.Id LEFT OUTER JOIN
                      dbo.业务备案_进口箱 AS C ON B.Id = C.Id LEFT OUTER JOIN
                      dbo.业务备案_内贸出港箱 AS D ON B.Id = D.Id



CREATE VIEW [dbo].[视图查询_押金]
AS
SELECT     A.日期, A.业务类型, A.费用项, A.金额, A.相关人, A.收付标志, B.凭证号,
                          (SELECT     dbo.Concatenate(DISTINCT 备注) AS Expr1
                            FROM          dbo.财务_凭证费用明细 AS X
                            WHERE      (凭证 = A.应收应付源)) AS 用途, B.备注, CASE 收付标志 WHEN 1 THEN '公司押给他人' ELSE '他人押在公司' END AS 类型
FROM         dbo.财务_应收应付款 AS A INNER JOIN
                      dbo.信息_业务类型 AS C ON A.业务类型 = C.代码 LEFT OUTER JOIN
                      dbo.财务_凭证 AS B ON A.应收应付源 = B.Id
WHERE     (A.费用项 = '002') AND (C.支出类别 = 205)


CREATE VIEW dbo.视图查询_业务备案_进口箱
AS
SELECT     C.Id, B.退滞箱费, A.最终滞箱费
FROM         dbo.业务备案_进口箱 AS C LEFT OUTER JOIN
                      dbo.视图信息_滞箱费减免进口箱_最终滞箱费 AS A ON C.Id = A.箱 LEFT OUTER JOIN
                      dbo.视图信息_滞箱费减免进口箱_退滞箱费 AS B ON C.Id = B.箱

CREATE VIEW [dbo].[视图查询_业务费用理论值小计_票费用项]
AS
SELECT     费用实体, 费用项, SUM(CASE WHEN 收付标志 = 1 THEN 金额 ELSE NULL END) AS 理论收金额, SUM(CASE WHEN 收付标志 = 2 THEN 金额 ELSE NULL
                       END) AS 理论付金额
FROM         dbo.财务_业务费用理论值
GROUP BY 费用实体, 费用项



CREATE VIEW [dbo].[视图查询_现金日记帐_余额]
AS
SELECT     ID, 币制, 源, 当前行数, 日期, 相关号码, 摘要, CASE t2.存取标志 WHEN 1 THEN t2.金额 ELSE NULL END AS 贷, 
                      CASE t2.存取标志 WHEN 2 THEN t2.金额 ELSE NULL END AS 借,
                          (SELECT     SUM(CASE WHEN t1.存取标志 = 2 THEN t1.金额 ELSE 0 - t1.金额 END) AS 目前为止合计
                            FROM          dbo.查询_现金日记帐_行数 AS t1
                            WHERE      (当前行数 <= t2.当前行数) AND (币制 = t2.币制)) AS 目前为止合计
FROM         dbo.查询_现金日记帐_行数 AS t2


CREATE VIEW [dbo].[视图查询_箱信息_进口]
AS
SELECT     A.箱号, A.箱型, A.重量, B.拉箱时间, B.卸货时间, B.卸货地, B.提箱地, B.还箱地, CASE WHEN B.卸货时间 IS NOT NULL 
                      THEN '承运结束' WHEN B.提箱时间 IS NOT NULL THEN '承运开始' ELSE '未承运' END AS 当前状态, C.货代自编号, A.封志号, C.Id, B.商检查验, 
                      f.所在地, B.货代提箱时间要求止, B.货代还箱时间要求止
FROM         dbo.业务备案_进口箱 AS B INNER JOIN
                      dbo.业务备案_普通箱 AS A ON B.Id = A.Id INNER JOIN
                      dbo.业务备案_普通票 AS C ON B.票 = C.Id LEFT OUTER JOIN
                      dbo.参数备案_人员单位 AS f ON f.编号 = B.卸货地


CREATE VIEW [dbo].[视图查询_箱信息_内贸出港]
AS
SELECT     A.箱号, A.箱型, A.重量, B.最终目的地, B.件数, B.回货箱号, B.装货地, B.拉箱时间, B.装货时间, B.提箱时间, B.还箱时间, 
                      CASE WHEN B.还箱时间 IS NOT NULL THEN '承运结束' WHEN B.提箱时间 IS NOT NULL THEN '对港承运开始' WHEN B.装货时间 IS NOT NULL 
                      THEN '已装货' ELSE '未承运' END AS 当前状态, C.货代自编号, A.封志号, A.品名, C.Id AS 票, B.车号, B.Id, C.提单号, D.联系方式
FROM         dbo.业务备案_内贸出港箱 AS B INNER JOIN
                      dbo.业务备案_普通箱 AS A ON B.Id = A.Id INNER JOIN
                      dbo.业务备案_普通票 AS C ON B.票 = C.Id LEFT OUTER JOIN
                      dbo.参数备案_人员单位 AS D ON B.最终目的地 = D.编号



CREATE VIEW [dbo].[视图查询_人员单位]
AS
SELECT     CASE substring(A.编号, 1, 2) WHEN '20' THEN '员工' ELSE '客户' END AS 源, A.编号, A.简称, A.全称, A.角色用途, A.联系方式, A.备注, A.IsActive, 
                      K.首次交往, K.信誉情况, K.简况, K.交往记录, Y.性别, Y.出生日期, Y.籍贯, Y.照片, Y.身份证号, Y.毕业院校, Y.学历, Y.专业, Y.政治面貌, Y.婚姻状况, 
                      Y.加入公司日期, Y.工作简历, Y.特长爱好
FROM         dbo.参数备案_人员单位 AS A LEFT OUTER JOIN
                      dbo.参数备案_人员单位_员工 AS Y ON Y.编号 = A.编号 LEFT OUTER JOIN
                      dbo.参数备案_人员单位_客户 AS K ON A.编号 = K.编号


CREATE VIEW [dbo].[视图查询_现金日记帐]
AS
SELECT     '现金支票提现' AS 源, Z.Id, Z.日期, 2 AS 存取标志, Z.金额, Y.币制, Z.摘要, Z.Id AS 信息主表Id, Z.票据号码 AS 相关号码
FROM         dbo.财务_支票 AS Z INNER JOIN
                      dbo.参数备案_银行账户 AS Y ON Z.银行账户 = Y.Id
WHERE     (Z.支票类型 = 1) AND (Z.支付凭证号 IS NULL) AND (Z.Submitted = 1)
UNION ALL
SELECT     '银行存取' AS 源, C.Id, C.日期, C.存取标志, C.金额, Y.币制, C.备注, C.Id AS 信息主表Id, C.编号 AS 相关号码
FROM         dbo.财务_银行存取款 AS C INNER JOIN
                      dbo.参数备案_银行账户 AS Y ON C.银行账户 = Y.Id
UNION ALL
SELECT     '换汇现金出款' AS 源, Id, 日期, 1 AS 存取标志, 出款数额 AS 金额, 出款币制 AS 币制, 摘要, Id AS 信息主表Id, 编号 AS 相关号码
FROM         dbo.财务_换汇 AS 换汇出
WHERE     (出款账户 IS NULL)
UNION ALL
SELECT     '换汇现金入款' AS 源, Id, 日期, 2 AS 存取标志, 入款数额 AS 金额, 入款币制 AS 币制, 摘要, Id AS 信息主表Id, 编号 AS 相关号码
FROM         dbo.财务_换汇 AS 换汇入
WHERE     (入款账户 IS NULL)
UNION ALL
SELECT     '凭证现金收付' AS 源, X.Id, P.日期, CASE X.收付标志 WHEN 1 THEN 2 ELSE 1 END AS 存取标志, X.金额, P.币制, P.摘要, P.Id AS 信息主表Id, 
                      P.凭证号 AS 相关号码
FROM         dbo.财务_凭证收支明细 AS X INNER JOIN
                      dbo.财务_凭证 AS P ON X.凭证 = P.Id
WHERE     (X.收付款方式 = 1) OR
                      (X.收付款方式 = 2) AND (X.收付标志 = 1)


CREATE VIEW [dbo].[视图查询_现金日记帐_行数]
AS
SELECT     p2.金额, p2.存取标志, p2.ID, p2.币制, p2.源, P2.日期, P2.相关号码, P2.摘要, ROW_NUMBER() OVER (PARTITION BY p2.币制
ORDER BY p2.日期, p2.ID) AS 当前行数
FROM         视图查询_现金日记帐 AS p2


CREATE VIEW [dbo].[视图信息_凭证费用项_动态]
AS
SELECT     编号, 名称, 1 AS 收付标志, 凭证收入类别 AS 凭证费用类别, IsActive
FROM         dbo.参数备案_费用项
WHERE     (凭证收入类别 IS NOT NULL)
UNION ALL
SELECT     编号, 名称, 2 AS 收付标志, 凭证支出类别 AS 凭证费用类别, IsActive
FROM         dbo.参数备案_费用项 AS 参数备案_费用项_2
WHERE     (凭证支出类别 IS NOT NULL)
UNION ALL
SELECT     编号, 名称, 1 AS Expr1, '203' AS Expr2, IsActive
FROM         dbo.参数备案_费用项 AS 参数备案_费用项_1
WHERE     (编号 = '012')
UNION ALL
SELECT     编号, 名称, 1 AS Expr1, '204' AS Expr2, IsActive
FROM         dbo.参数备案_费用项 AS 参数备案_费用项_1
WHERE     (编号 = '012')
UNION ALL
SELECT     编号, 名称, 1 AS Expr1, '205' AS Expr2, IsActive
FROM         dbo.参数备案_费用项 AS 参数备案_费用项_1
WHERE     (编号 = '012')
UNION ALL
SELECT     编号, 名称, 2 AS Expr1, '203' AS Expr2, IsActive
FROM         dbo.参数备案_费用项 AS 参数备案_费用项_1
WHERE     (编号 = '012')
UNION ALL
SELECT     编号, 名称, 2 AS Expr1, '204' AS Expr2, IsActive
FROM         dbo.参数备案_费用项 AS 参数备案_费用项_1
WHERE     (编号 = '012')
UNION ALL
SELECT     编号, 名称, 2 AS Expr1, '205' AS Expr2, IsActive
FROM         dbo.参数备案_费用项 AS 参数备案_费用项_1
WHERE     (编号 = '012')


CREATE VIEW [dbo].[报表_宁波新概念出运清单_给委托人]
AS
SELECT     a.提单号, b.箱型, b.装货时间, b.装货地, b.箱号, b.封志号, b.车号, b.回货箱号, b.品名, b.最终目的地, a.开航日期, a.预计到港时间, a.船名航次, 
                      a.委托人, '门到门' AS 服务要求
FROM         dbo.视图查询_票信息_内贸出港 AS a INNER JOIN
                      dbo.视图查询_箱信息_内贸出港 AS b ON a.Id = b.票




CREATE VIEW [dbo].[报表_营业外收入明细]
AS
SELECT     Id, 费用项, 收付标志, 相关人, 费用实体, 凭证费用明细, 金额, 收款金额, 付款金额, 费用类别, 费用实体类型, 入账日期 AS 日期, 相关号, 备注, 凭证Id, 
                      小类, 大类, 凭证号, 经手人
FROM         dbo.视图查询_非业务费用明细
WHERE     (大类 = '营业外收入') AND (收付标志 = 1)



CREATE VIEW [dbo].[报表_凭证]
AS
SELECT     凭证类别, 相关人 AS 相关人编号, 审核状态, 收支状态, 是否作废, 备注, 摘要, 会计金额, 审核人 AS 审核人编号, 会计 AS 会计编号, 出纳 AS 出纳编号, 
                      币制 AS 币制编号, 数额, dbo.ConvertToChineseMoney(数额) AS 大写金额, 凭证号, 日期
FROM         dbo.财务_凭证


CREATE VIEW [dbo].[视图信息_凭证费用类别]
AS
SELECT     1 AS 收付标志, 代码, 名称, IsActive, 凭证用途分类
FROM         dbo.信息_凭证费用类别
WHERE     (收 = 1)
UNION ALL
SELECT     2 AS 收付标志, 代码, 名称, IsActive, 凭证用途分类
FROM         dbo.信息_凭证费用类别 AS 信息_凭证费用类别_1
WHERE     (付 = 1)


CREATE VIEW [dbo].[报表_宁波新概念出运清单_给船公司]
AS
SELECT     b.箱号, b.封志号, b.箱型, b.件数, b.重量, b.品名, a.条款, a.目的港, a.预计开航日期, a.船名航次 AS 预配船名航次, a.提单号 AS 预配提单号
FROM         dbo.视图查询_票信息_内贸出港 AS a INNER JOIN
                      dbo.视图查询_箱信息_内贸出港 AS b ON a.Id = b.票



CREATE VIEW [dbo].[报表_业务盈亏预估表]
AS
SELECT     收付标志, 费用实体, SUM(金额小计) AS 金额小计, SUM(已确认金额) AS 已确认金额, SUM(已分离金额) AS 已分离金额, SUM(未分离金额) AS 未分离金额, 
                      票时间, 费用实体类型, 大类
FROM         dbo.视图查询_业务费用小计
GROUP BY 票时间, 收付标志, 费用实体, 费用实体类型, 大类


CREATE VIEW [dbo].[视图信息_坏账业务类型_动态]
AS
SELECT     A.代码, A.类型, C.编号 AS 费用项编号
FROM         dbo.信息_业务类型 AS A INNER JOIN
                      dbo.信息_凭证费用类别 AS B ON A.收入类别 = B.代码 INNER JOIN
                      dbo.参数备案_费用项 AS C ON B.代码 = C.凭证收入类别 AND (C.编号 = '000' OR
                      C.编号 = '001')
UNION ALL
SELECT     代码, 类型, '002' AS 费用项编号
FROM         dbo.信息_业务类型 AS A
WHERE     (代码 >= 111) AND (代码 <= 132)


CREATE VIEW [dbo].[报表_内贸结算明细]
AS
SELECT     TOP (100) PERCENT a.编号 AS 对账单, a.关账日期, b.委托人, b.开航日期, b.货代自编号, b.箱量, b.船名航次, b.提单号, b.目的港, 
                      a.费用项 AS 费用项编号, c.名称 AS 费用项, a.金额, b.对上备注 AS 备注, d.箱号, d.箱型, d.封志号, d.装货地,
                          (SELECT     TOP (1) 金额
                            FROM          dbo.财务_费用
                            WHERE      (费用实体 = b.Id) AND (箱 = d.Id) AND (费用项 = 101) AND (收付标志 = 1)) AS 包干费, d.最终目的地, a.起始日期
FROM         dbo.视图查询_对账单_票费用项金额 AS a INNER JOIN
                      dbo.视图查询_票信息_内贸出港 AS b ON a.费用实体 = b.Id INNER JOIN
                      dbo.参数备案_费用项 AS c ON c.编号 = a.费用项 INNER JOIN
                      dbo.视图查询_箱信息_内贸出港 AS d ON b.Id = d.票
ORDER BY b.货代自编号


CREATE VIEW [dbo].[报表_实付佣金明细]
AS
SELECT     金额, 入账日期 AS 日期, 凭证号, 备注, 相关人, 经手人
FROM         dbo.视图查询_业务费用明细
WHERE     (费用项 = 143)


CREATE VIEW [dbo].[视图信息_固定资产折旧_固定资产]
AS
SELECT     A.Id, A.分类, A.购入时间, A.简介, A.使用年限, A.购入金额, A.月折旧额, A.备注, B.编号,
                          (SELECT     SUM(金额) AS Expr1
                            FROM          dbo.财务_费用
                            WHERE      (费用实体 = A.Id)) AS 累计折旧
FROM         dbo.财务_固定资产 AS A INNER JOIN
                      dbo.财务_费用实体 AS B ON A.Id = B.Id
WHERE     (A.状态 = 1)


CREATE VIEW [dbo].[报表_进口转关运输货物核放单]
AS
SELECT     a.货代自编号, NULL AS 转关申报单编号, '宁波易可报关有限公司' AS 申请人, NULL AS 进境运输工具名称, a.提单号 AS 提运单号, a.箱量 AS 数量, 
                      a.总重量 AS 重量, a.船名航次, a.承运人, b.箱号, b.封志号, b.箱型, a.到港时间 AS 进境日期, a.报关员, a.转关指运地 AS 指运地, a.件数, NULL 
                      AS 预计运抵时间, NULL AS 经办关员, NULL AS 境内运输工具名称, NULL AS 境内运输工具编号
FROM         dbo.视图查询_票信息_进口 AS a LEFT OUTER JOIN
                      dbo.视图查询_箱信息_进口 AS b ON a.货代自编号 = b.货代自编号


CREATE VIEW [dbo].[报表_其它开支明细]
AS
SELECT     费用项, 金额, 入账日期 AS 日期, 备注, 凭证号, 经手人
FROM         dbo.视图查询_非业务费用明细
WHERE     (大类 = '其他业务')


CREATE VIEW [dbo].[视图信息_费用实体类型_业务]
AS
SELECT     11 AS 费用实体类型, '进口' AS 业务
UNION ALL
SELECT     13 AS 费用实体类型, '出口' AS 业务
UNION ALL
SELECT     15 AS 费用实体类型, '内贸出港' AS 业务
UNION ALL
SELECT     41 AS 费用实体类型, '出口只票不箱' AS 业务
UNION ALL
SELECT     42 AS 费用实体类型, '代领通关单' AS 业务
UNION ALL
SELECT     45 AS 费用实体类型, '进口其他业务' AS 业务


CREATE VIEW [dbo].[视图信息_费用实体类型_非业务]
AS
SELECT     21 AS 费用实体类型, '承兑汇票' AS 非业务
UNION ALL
SELECT     23 AS 费用实体类型, '发票' AS 非业务
UNION ALL
SELECT     24 AS 费用实体类型, '固定资产' AS 非业务
UNION ALL
SELECT     25 AS 费用实体类型, '工资单' AS 非业务
UNION ALL
SELECT     26 AS 费用实体类型, '借贷' AS 非业务
UNION ALL
SELECT     27 AS 费用实体类型, '押金' AS 非业务
UNION ALL
SELECT     28 AS 费用实体类型, '套现' AS 非业务
UNION ALL
SELECT     30 AS 费用实体类型, '投资' AS 非业务
UNION ALL
SELECT     31 AS 费用实体类型, '小件资产' AS 非业务
UNION ALL
SELECT     32 AS 费用实体类型, '成本发票' AS 非业务
UNION ALL
SELECT     37 AS 费用实体类型, '其他业务' AS 非业务


CREATE VIEW [dbo].[报表_货代报关部委托运输联系单_进口]
AS
SELECT     a.货代自编号, a.委托时间, a.委托人, a.提单号, a.箱量, a.总重量, a.船名航次, a.合同号, a.品名 AS 货物名称, a.对下备注 AS 备注, a.到港时间, b.箱号, 
                      b.箱型, b.重量, b.卸货地 AS 收货人, b.提箱地, b.所在地 AS 卸货地, b.货代提箱时间要求止 AS 疏港期限, b.货代还箱时间要求止 AS 还箱期限, 
                      b.商检查验, c.放行时间 AS 承运时间_始, DATEADD(day, c.要求承运天数, c.放行时间) AS 承运时间_止, d.联系方式
FROM         dbo.视图查询_票信息_进口 AS a INNER JOIN
                      dbo.视图查询_箱信息_进口 AS b ON a.Id = b.Id INNER JOIN
                      dbo.业务备案_进口票 AS c ON b.Id = c.Id INNER JOIN
                      dbo.参数备案_人员单位 AS d ON a.委托人 = d.编号


CREATE VIEW [dbo].[视图信息_对账单_票]
AS
SELECT     a.对账单, a.费用实体, b.对账单类型, b.费用项, b.编号 AS 对账单号, c.货代自编号
FROM         dbo.财务_费用 AS a INNER JOIN
                      dbo.财务_对账单 AS b ON a.对账单 = b.Id INNER JOIN
                      dbo.业务备案_普通票 AS c ON a.费用实体 = c.Id
WHERE     (a.对账单 IS NOT NULL)
GROUP BY a.对账单, a.费用实体, b.对账单类型, b.费用项, b.编号, c.货代自编号


CREATE VIEW [dbo].[报表_管理费用开支明细]
AS
SELECT     Id, 费用项, 收付标志, 相关人, 费用实体, 凭证费用明细, 金额, 收款金额, 付款金额, 费用类别, 费用实体类型, 入账日期 AS 日期, 相关号, 备注, 凭证Id, 
                      小类, 大类, 凭证号, 经手人, '总公司' AS 部门
FROM         dbo.视图查询_非业务费用明细
WHERE     (大类 = '管理费用') AND (收付标志 = 2)



CREATE VIEW [dbo].[报表_对账单]
AS
SELECT     b.委托人, a.金额, a.编号 AS 对账单, b.货代自编号, b.提单号, b.合同号, b.货物类别 AS 货名, b.委托时间, a.费用项 AS 费用项编号, c.名称 AS 费用项, 
                      b.箱量, b.总重量, b.船公司, b.船名航次, a.关账日期, a.业务类型, a.常规额外, a.对账单类型, a.起始日期, b.放行时间, b.到港时间, b.卸箱地, b.单证晚到, 
                      b.免箱联系货主时间, b.最终免箱标志, b.对上备注 AS 备注
FROM         dbo.视图查询_对账单_票费用项金额 AS a LEFT OUTER JOIN
                      dbo.视图查询_票信息_进口 AS b ON a.费用实体 = b.Id LEFT OUTER JOIN
                      dbo.参数备案_费用项 AS c ON a.费用项 = c.编号


CREATE VIEW [dbo].[报表_非业务费用明细_他人投资公司增减明细]
AS
SELECT     入账日期 AS 日期, 相关人, 类型, 投资金额, 撤资金额, 备注, 相关号, 凭证号, 简介
FROM         dbo.视图查询_非业务费用明细_投资
WHERE     (类型 = '他人投资公司')


CREATE VIEW [dbo].[视图费用项_业务]
AS
SELECT     A.编号, A.名称, A.现有费用实体类型, A.收, A.付, A.票, A.箱, B.小类, B.大类, B.代码 AS 费用类别, A.支出类别, A.收入类别, 
                      CASE A.票 WHEN 1 THEN '票,' ELSE '' END + CASE A.箱 WHEN 1 THEN '箱,' ELSE '' END AS 票箱, A.SeqNo, A.IsActive
FROM         dbo.参数备案_费用项 AS A INNER JOIN
                      dbo.信息_费用类别 AS B ON A.收入类别 = B.代码
WHERE     (B.大类 = '业务额外') OR
                      (B.大类 = '业务常规') OR
                      (B.大类 = '业务其他')


CREATE VIEW [dbo].[报表_非业务费用明细_公司投资他人增减明细]
AS
SELECT     入账日期 AS 日期, 相关人, 类型, 投资金额, 撤资金额, 备注, 相关号, 凭证号, 简介
FROM         dbo.视图查询_非业务费用明细_投资
WHERE     (类型 = '公司投资他人')


CREATE VIEW [dbo].[视图费用项_非业务]
AS
SELECT DISTINCT A.编号, A.名称, A.现有费用实体类型, A.收, A.付, A.支出类别, A.收入类别, A.IsActive
FROM         dbo.参数备案_费用项 AS A INNER JOIN
                      dbo.信息_费用类别 AS B ON A.收入类别 = B.代码 OR A.支出类别 = B.代码
WHERE     (NOT (B.大类 = '业务额外' OR
                      B.大类 = '业务常规'))


CREATE VIEW dbo.视图查询_费用信息_最大时间
AS
SELECT     费用信息, MAX(最大时间) AS 最大时间
FROM         (SELECT     CAST(费用信息 AS uniqueidentifier) AS 费用信息, MAX(ISNULL(Updated, Created)) AS 最大时间
                       FROM          dbo.财务_费用 AS a
                       GROUP BY 费用信息
                       UNION ALL
                       SELECT     CAST(Id AS uniqueidentifier) AS 费用信息, MAX(ISNULL(Updated, Created)) AS 最大时间
                       FROM         dbo.财务_费用信息 AS a
                       GROUP BY Id) AS b
GROUP BY 费用信息

CREATE VIEW [dbo].[视图信息_收付款方式]
AS
SELECT     CAST(1 AS int) AS 编号, '现金' AS 名称
UNION ALL
SELECT     CAST(2 AS int) AS 编号, '现金支票' AS 名称
UNION ALL
SELECT     CAST(3 AS int) AS 编号, '转账支票' AS 名称
UNION ALL
SELECT     CAST(4 AS int) AS 编号, '银行承兑汇票' AS 名称
UNION ALL
SELECT     CAST(5 AS int) AS 编号, '银行本票汇票' AS 名称
UNION ALL
SELECT     CAST(6 AS int) AS 编号, '银行收付' AS 名称
UNION ALL
SELECT     CAST(7 AS int) AS 编号, '电汇' AS 名称



CREATE VIEW [dbo].[视图信息_财务费用_车队承担]
AS
SELECT     a.相关人, SUM(CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - a.金额 END) AS 车队承担, a.费用实体, a.箱, a.费用项
FROM         dbo.财务_费用 AS a INNER JOIN
                      dbo.视图查询_票信息_委托人 AS b ON a.费用实体 = b.Id AND a.相关人 = b.承运人 INNER JOIN
                      dbo.信息_费用类别 AS c ON a.费用类别 = c.代码
WHERE     (c.大类 = '业务额外')
GROUP BY a.相关人, a.费用实体, a.箱, a.费用项


CREATE VIEW [dbo].[视图信息_凭证业务类型_动态]
AS
SELECT     1 AS 收付标志, 代码, 类型, 收入类别 AS 凭证费用类别, IsActive
FROM         dbo.信息_业务类型
WHERE     (收入类别 IS NOT NULL)
UNION ALL
SELECT     2 AS 收付标志, 代码, 类型, 支出类别 AS 凭证费用类别, IsActive
FROM         dbo.信息_业务类型 AS 信息_业务类型_1
WHERE     (收入类别 IS NOT NULL)
UNION ALL
SELECT     1 AS 收付标志, 代码, 类型, '103' AS 凭证费用类别, IsActive
FROM         dbo.信息_业务类型 AS 信息_业务类型_1
WHERE     (代码 = '111') OR
                      (代码 = '112') OR
                      (代码 = '113') OR
                      (代码 = '121') OR
                      (代码 = '122') OR
                      (代码 = '131') OR
                      (代码 = '132')
UNION ALL
SELECT     2 AS 收付标志, 代码, 类型, '104' AS 凭证费用类别, IsActive
FROM         dbo.信息_业务类型 AS 信息_业务类型_1
WHERE     (代码 = '111') OR
                      (代码 = '112') OR
                      (代码 = '113') OR
                      (代码 = '121') OR
                      (代码 = '122') OR
                      (代码 = '131') OR
                      (代码 = '132')
UNION ALL
SELECT     2 AS 收付标志, 代码, 类型, '999' AS 凭证费用类别, IsActive
FROM         dbo.信息_业务类型 AS 信息_业务类型_1
WHERE     (代码 = '11') OR
                      (代码 = '13') OR
                      (代码 = '15') OR
                      (代码 = '45')


CREATE VIEW [dbo].[视图信息_凭证号]
AS
SELECT     X.Id, P.凭证号, '凭证费用明细' AS 源
FROM         dbo.财务_凭证费用明细 AS X INNER JOIN
                      dbo.财务_凭证 AS P ON X.凭证 = P.Id
UNION ALL
SELECT     X.Id, P.凭证号, '凭证收支明细' AS 源
FROM         dbo.财务_凭证收支明细 AS X INNER JOIN
                      dbo.财务_凭证 AS P ON X.凭证 = P.Id
UNION ALL
SELECT     Id, 凭证号, '凭证' AS 源
FROM         dbo.财务_凭证 AS P


CREATE VIEW [dbo].[报表_宁波新概念派车单]
AS
SELECT     TOP (100) PERCENT
                          (SELECT     SUM(金额) AS 拖车费
                            FROM          dbo.财务_费用
                            WHERE      (箱 = b.Id) AND (费用实体 = c.Id) AND (费用项 = 184) AND (收付标志 = 2)) AS 拖车费, b.提单号, b.箱号, b.封志号, b.箱型, b.最终目的地, 
                      b.联系方式, c.开航日期, c.预计到港时间, c.船名航次
FROM         dbo.视图查询_票信息_内贸出港 AS c INNER JOIN
                      dbo.视图查询_箱信息_内贸出港 AS b ON c.Id = b.票


CREATE VIEW [dbo].[视图信息_费用信息_额外]
AS
SELECT     B.货代自编号, C.费用类别, A.业务类型, A.Id, A.备注, A.付款对账凭证状态, A.收款对账凭证状态, A.填全状态, A.警示标志, A.收款创建时间, 
                      A.收款修改时间, A.凭证号, A.对账单号, A.创建天数, A.Submitted, B.提单号, A.Created, B.Id AS 费用实体
FROM         dbo.财务_费用信息 AS A INNER JOIN
                      dbo.业务备案_普通票 AS B ON A.票 = B.Id INNER JOIN
                      dbo.视图费用项_业务 AS C ON A.费用项 = C.编号 AND C.大类 = '业务额外'


CREATE VIEW [dbo].[视图信息_箱票]
AS
SELECT     Id, 票, 11 AS 业务类型
FROM         dbo.业务备案_进口箱
UNION ALL
SELECT     Id, 票, 45 AS 业务类型
FROM         dbo.业务备案_进口其他业务箱
UNION ALL
SELECT     Id, 票, 15 AS 业务类型
FROM         dbo.业务备案_内贸出港箱


CREATE VIEW [dbo].[视图信息_财务费用_对外付款]
AS
SELECT     a.相关人, SUM(CASE WHEN a.收付标志 = 2 THEN a.金额 ELSE 0 - a.金额 END) AS 对外付款, a.费用实体, a.费用项, a.箱
FROM         dbo.财务_费用 AS a INNER JOIN
                      dbo.视图查询_票信息_委托人 AS b ON a.费用实体 = b.Id INNER JOIN
                      dbo.信息_费用类别 AS c ON a.费用类别 = c.代码
WHERE     (a.相关人 NOT IN (b.承运人, b.委托人)) AND (c.大类 = '业务额外')
GROUP BY a.相关人, a.费用实体, a.费用项, a.箱


CREATE VIEW [dbo].[视图信息_财务费用_委托人承担]
AS
SELECT     a.相关人, SUM(CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - a.金额 END) AS 委托人承担, a.费用项, a.箱, a.费用实体
FROM         dbo.财务_费用 AS a INNER JOIN
                      dbo.业务备案_普通票 AS b ON a.费用实体 = b.Id AND a.相关人 = b.委托人 INNER JOIN
                      dbo.信息_费用类别 AS c ON a.费用类别 = c.代码
WHERE     (c.大类 = '业务额外')
GROUP BY a.相关人, a.费用项, a.箱, a.费用实体


CREATE VIEW [dbo].[视图信息_财务费用信息_车队承担]
AS
SELECT     a.相关人, a.费用信息, SUM(CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - a.金额 END) AS 车队承担, a.费用实体, a.费用项, a.凭证费用明细, 
                      a.对账单
FROM         dbo.财务_费用 AS a INNER JOIN
                      dbo.视图查询_票信息_委托人 AS b ON a.费用实体 = b.Id AND a.相关人 = b.承运人 INNER JOIN
                      dbo.信息_费用类别 AS c ON a.费用类别 = c.代码
WHERE     (c.大类 = '业务额外')
GROUP BY a.相关人, a.费用信息, a.费用实体, a.费用项, a.凭证费用明细, a.对账单


CREATE VIEW [dbo].[视图信息_应收应付源]
AS
SELECT     Id, 编号, 
                      CASE WHEN 对账单类型 = 2 THEN '应收对账单' WHEN 对账单类型 = 3 THEN '应付对账单' WHEN 对账单类型 = 4 THEN '应收调节款' WHEN 对账单类型
                       = 5 THEN '坏账' END AS 源
FROM         dbo.财务_对账单
UNION ALL
SELECT     Id, 凭证号 AS 编号, '凭证' AS 源
FROM         dbo.财务_凭证
UNION ALL
SELECT     Id, 编号, '调节款' AS 源
FROM         dbo.财务_调节款


CREATE VIEW [dbo].[报表_修洗箱费]
AS
SELECT     B.委托人, B.货代自编号, B.提单号, B.船公司, B.箱量, A.箱号, A.还箱时间, SUM(C.金额) AS 修洗箱费, A.Id, D.编号 AS 对账单, E.委托人承担, 
                      E.车队承担, E.对外付款, E.自己承担, D.关账日期
FROM         dbo.视图信息_箱号 AS A INNER JOIN
                      dbo.视图查询_票信息_委托人 AS B ON A.票 = B.Id INNER JOIN
                      dbo.财务_费用 AS C ON A.Id = C.箱 INNER JOIN
                      dbo.财务_费用信息 AS E ON C.费用信息 = E.Id LEFT OUTER JOIN
                      dbo.财务_对账单 AS D ON C.对账单 = D.Id
WHERE     (C.费用项 = 165) AND (C.收付标志 = 2)
GROUP BY C.费用实体, C.箱, B.委托人, B.货代自编号, B.船公司, B.箱量, B.提单号, A.箱号, A.还箱时间, A.票, A.Id, D.编号, E.委托人承担, E.车队承担, E.对外付款,
                       E.自己承担, D.关账日期


CREATE VIEW [dbo].[视图信息_财务费用信息_对外付款]
AS
SELECT     a.相关人, a.费用信息, SUM(CASE WHEN a.收付标志 = 2 THEN a.金额 ELSE 0 - a.金额 END) AS 对外付款, a.费用实体, a.费用项, a.对账单, 
                      a.凭证费用明细
FROM         dbo.财务_费用 AS a INNER JOIN
                      dbo.视图查询_票信息_委托人 AS b ON a.费用实体 = b.Id INNER JOIN
                      dbo.信息_费用类别 AS c ON a.费用类别 = c.代码
WHERE     (a.相关人 NOT IN (b.承运人, b.委托人)) AND (c.大类 = '业务额外') OR
                      (a.相关人 = b.承运人) AND (a.收付标志 = 2) AND (a.金额 > 0) AND (c.大类 = '业务额外')
GROUP BY a.相关人, a.费用信息, a.费用实体, a.费用项, a.对账单, a.凭证费用明细


CREATE VIEW [dbo].[视图信息_财务费用信息_退滞箱费]
AS
SELECT     a.相关人, a.费用信息, SUM(CASE WHEN A.收付标志 = 1 AND 金额 > 0 THEN A.金额 WHEN A.收付标志 = 2 AND 金额 < 0 THEN - A.金额 ELSE 0 END) 
                      AS 退滞箱费, a.费用实体, a.费用项, a.对账单, a.凭证费用明细
FROM         dbo.财务_费用 AS a INNER JOIN
                      dbo.业务备案_普通票 AS B ON a.费用实体 = B.Id AND a.相关人 <> B.委托人 INNER JOIN
                      dbo.业务备案_进口票 AS D ON D.Id = B.Id AND a.相关人 <> D.承运人
WHERE     (a.收付标志 = 1) AND (a.金额 > 0) AND (a.费用项 = 167) OR
                      (a.收付标志 = 2) AND (a.金额 < 0) AND (a.费用项 = 167)
GROUP BY a.相关人, a.费用信息, a.费用实体, a.费用项, a.对账单, a.凭证费用明细


CREATE VIEW [dbo].[视图信息_财务费用信息_委托人承担]
AS
SELECT     a.相关人, a.费用信息, SUM(CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - a.金额 END) AS 委托人承担, a.费用实体, a.费用项, a.费用类别, 
                      a.对账单, a.凭证费用明细
FROM         dbo.财务_费用 AS a INNER JOIN
                      dbo.业务备案_普通票 AS b ON a.费用实体 = b.Id AND a.相关人 = b.委托人 INNER JOIN
                      dbo.信息_费用类别 AS c ON a.费用类别 = c.代码
WHERE     (c.大类 = '业务额外')
GROUP BY a.相关人, a.费用信息, a.费用实体, a.费用项, a.费用类别, a.对账单, a.凭证费用明细


CREATE VIEW dbo.视图查询_票费用项_业务盈亏
AS
SELECT     *
FROM         (SELECT     id
                       FROM          业务备案_普通票) AS a CROSS apply dbo.函数查询_业务费用小计_票费用项(a.id)

CREATE VIEW [dbo].[视图信息_财务费用信息_最终滞箱费]
AS
SELECT     a.相关人, a.费用信息, SUM(CASE WHEN A.收付标志 = 2 AND 金额 > 0 THEN A.金额 WHEN A.收付标志 = 1 AND 金额 < 0 THEN - A.金额 ELSE 0 END) 
                      AS 最终滞箱费, a.费用实体, a.费用项, a.对账单, a.凭证费用明细
FROM         dbo.财务_费用 AS a INNER JOIN
                      dbo.业务备案_普通票 AS B ON a.费用实体 = B.Id AND a.相关人 <> B.委托人 INNER JOIN
                      dbo.业务备案_进口票 AS D ON D.Id = B.Id AND a.相关人 <> D.承运人
WHERE     (a.费用项 = 167) AND (a.收付标志 = 1) AND (a.金额 < 0) OR
                      (a.费用项 = 167) AND (a.收付标志 = 2) AND (a.金额 > 0)
GROUP BY a.相关人, a.费用信息, a.费用实体, a.费用项, a.对账单, a.凭证费用明细


CREATE VIEW [dbo].[视图信息_滞箱费减免进口箱_车队承担]
AS
SELECT     a.相关人, a.费用项, a.金额, a.费用实体, a.箱, SUM(CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - a.金额 END) AS 车队承担
FROM         dbo.财务_费用 AS a INNER JOIN
                      dbo.业务备案_进口票 AS b ON a.费用实体 = b.Id AND a.相关人 = b.承运人
WHERE     (a.费用项 = 167)
GROUP BY a.相关人, a.金额, a.费用实体, a.箱, a.费用项


CREATE VIEW [dbo].[报表_贷款明细]
AS
SELECT     日期, 结算期限, 业务类型, 费用项, 金额, 相关人, 收付标志, 凭证号, 收付款方式, 用途, 备注
FROM         dbo.视图查询_借贷
WHERE     (收付标志 = 2) AND (金额 > 0)



CREATE VIEW [dbo].[视图信息_滞箱费减免进口箱_委托人承担]
AS
SELECT     a.相关人, a.费用项, a.费用实体, a.箱, SUM(CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - a.金额 END) AS 委托人承担
FROM         dbo.财务_费用 AS a INNER JOIN
                      dbo.业务备案_普通票 AS b ON a.费用实体 = b.Id AND a.相关人 = b.委托人
WHERE     (a.费用项 = 167)
GROUP BY a.相关人, a.费用实体, a.箱, a.费用项


CREATE VIEW [dbo].[视图信息_滞箱费减免进口箱_无法归属NO]
AS
SELECT     a.相关人, a.费用项, a.费用实体, a.箱, c.船公司, SUM(CASE WHEN a.收付标志 = 1 THEN a.金额 ELSE 0 - a.金额 END) AS 无法归属
FROM         dbo.财务_费用 AS a INNER JOIN
                      dbo.业务备案_进口票 AS b ON a.费用实体 = b.Id LEFT OUTER JOIN
                      dbo.业务备案_普通票 AS c ON a.费用实体 = c.Id
WHERE     (a.相关人 NOT IN (b.承运人, c.委托人, c.船公司)) AND (a.费用项 IN (167, 168))
GROUP BY a.相关人, a.费用实体, a.箱, c.船公司, a.费用项



CREATE VIEW [dbo].[视图信息_滞箱费减免进口箱_退滞箱费]
AS
SELECT     a.相关人, a.费用项, a.费用实体, a.箱, B.船公司, SUM(CASE WHEN A.收付标志 = 1 AND 金额 > 0 THEN A.金额 WHEN A.收付标志 = 2 AND 
                      金额 < 0 THEN - A.金额 ELSE 0 END) AS 退滞箱费
FROM         dbo.财务_费用 AS a INNER JOIN
                      dbo.业务备案_普通票 AS B ON a.费用实体 = B.Id AND a.相关人 <> B.委托人 INNER JOIN
                      dbo.业务备案_进口票 AS D ON D.Id = B.Id AND a.相关人 <> D.承运人
WHERE     (a.费用项 = 167) AND (a.收付标志 = 1) AND (a.金额 > 0) OR
                      (a.费用项 = 167) AND (a.收付标志 = 2) AND (a.金额 < 0)
GROUP BY a.相关人, a.费用实体, a.箱, B.船公司, a.费用项


CREATE VIEW [dbo].[报表_借出款明细]
AS
SELECT     日期, 结算期限, 业务类型, 费用项, 金额, 相关人, 收付标志, 凭证号, 收付款方式, 用途, 备注
FROM         dbo.视图查询_借贷
WHERE     (收付标志 = 1) AND (金额 > 0)



CREATE VIEW [dbo].[视图信息_滞箱费减免进口箱_最终滞箱费]
AS
SELECT     a.相关人, a.费用项, a.费用实体, a.箱, B.船公司, SUM(CASE WHEN A.收付标志 = 2 AND 金额 > 0 THEN A.金额 WHEN A.收付标志 = 1 AND 
                      金额 < 0 THEN - A.金额 ELSE 0 END) AS 最终滞箱费
FROM         dbo.财务_费用 AS a INNER JOIN
                      dbo.业务备案_普通票 AS B ON a.费用实体 = B.Id AND a.相关人 <> B.委托人 INNER JOIN
                      dbo.业务备案_进口票 AS D ON D.Id = B.Id AND a.相关人 <> D.承运人
WHERE     (a.费用项 = 167) AND (a.收付标志 = 1) AND (a.金额 < 0) OR
                      (a.费用项 = 167) AND (a.收付标志 = 2) AND (a.金额 > 0)
GROUP BY a.相关人, a.费用实体, a.箱, B.船公司, a.费用项


CREATE VIEW [dbo].[报表_滞箱费]
AS
SELECT     b.委托人, b.船公司, a.货代自编号, a.对账单, a.金额 AS 滞箱费, a.委托人承担, a.车队承担, a.对外付款, a.自己承担, b.Id, a.关账日期
FROM         dbo.视图查询_票费用项_金额 AS a INNER JOIN
                      dbo.业务备案_普通票 AS b ON a.票 = b.Id
WHERE     (a.费用项 = 167) AND (a.收付标志 = 2)


CREATE VIEW [dbo].[报表_资金明细_承兑汇票]
AS
SELECT     所属银行, 来源, 票号, 金额, 托收贴现, 到期时间, 返回时间, 日期
FROM         dbo.视图查询_承兑汇票_凭证收付
WHERE     (收付标志 = 1) AND (票据号码 NOT IN
                          (SELECT     票据号码
                            FROM          dbo.视图查询_承兑汇票_凭证收付 AS 视图查询_承兑汇票_凭证收付_1
                            WHERE      (收付标志 = 2)))



CREATE VIEW [dbo].[报表_资金流水]
AS
SELECT     a.日期, a.币制, (CASE WHEN a.收付标志 = 1 THEN a.金额 END) AS 收入金额, (CASE WHEN a.收付标志 = 2 THEN a.金额 END) AS 支出金额, 
                      a.收付款方式, a.费用项, a.备注, a.相关人, b.凭证号, b.业务类型
FROM         dbo.视图查询_资金流水明细 AS a LEFT OUTER JOIN
                      dbo.报表_凭证费用明细 AS b ON a.相关号码 = b.凭证号 AND a.金额 = b.金额



CREATE VIEW [dbo].[视图查询_非业务费用明细]
AS
SELECT     F.Id, F.费用项, F.收付标志, F.相关人, F.费用实体, F.凭证费用明细, F.金额, CASE F.收付标志 WHEN 1 THEN F.金额 ELSE NULL END AS 收款金额, 
                      CASE F.收付标志 WHEN 2 THEN F.金额 ELSE NULL END AS 付款金额, F.费用类别, A.费用实体类型, CASE WHEN f.费用项 = '352' OR
                      f.费用项 = '387' THEN H.关账日期 ELSE E.日期 END AS 入账日期, A.相关号, F.备注, E.Id AS 凭证Id, G.小类, G.大类, E.凭证号, E.相关人 AS 经手人, 
                      F.对账单, H.编号 AS 对账单号, H.Submitted AS 对账单提交
FROM         dbo.财务_凭证费用明细 AS D LEFT OUTER JOIN
                      dbo.财务_凭证 AS E ON D.凭证 = E.Id RIGHT OUTER JOIN
                      dbo.视图信息_费用实体_非业务 AS A RIGHT OUTER JOIN
                      dbo.信息_费用类别 AS G INNER JOIN
                      dbo.财务_费用 AS F ON G.代码 = F.费用类别 LEFT OUTER JOIN
                      dbo.财务_对账单 AS H ON F.对账单 = H.Id ON A.Id = F.费用实体 ON D.Id = F.凭证费用明细
WHERE     (F.费用类别 >= 40) AND (F.费用类型 = 11)


CREATE VIEW [dbo].[报表_资金明细_银行存款_行数]
AS
SELECT     ROW_NUMBER() OVER (partition BY 开户行, 存取标志, 账号
ORDER BY 日期) AS 当前行数, t1.存取标志, t2.开户行 AS 开户银行, t2.币制, t2.账号, t1.日期, t1.金额, t1.摘要 AS 备注, t1.相关号码
FROM         dbo.视图查询_银行日记帐 AS t1 LEFT OUTER JOIN
                      dbo.参数备案_银行账户 AS t2 ON t1.银行账户 = t2.Id
GROUP BY t1.存取标志, t2.开户行, t2.币制, t2.账号, t1.日期, t1.金额, t1.摘要, t1.相关号码




CREATE VIEW [dbo].[报表_资金明细_银行存款]
AS
SELECT     t2.开户行 AS 开户银行, t2.币制, t2.账号, CASE WHEN t1.存取标志 = 1 THEN t1.金额 ELSE 0 - t1.金额 END AS 本月增减, t1.日期, t1.摘要 AS 备注
FROM         dbo.视图查询_银行日记帐 AS t1 LEFT OUTER JOIN
                      dbo.参数备案_银行账户 AS t2 ON t1.银行账户 = t2.Id
GROUP BY t1.存取标志, t2.开户行, t2.币制, t2.账号, t1.日期, t1.金额, t1.摘要, t1.相关号码





CREATE VIEW [dbo].[视图查询_非业务费用明细_投资]
AS
SELECT     A.入账日期, A.相关人, CASE WHEN (收付标志 = 1 AND 费用项 = 321) OR
                      (收付标志 = 2 AND 费用项 = 322) THEN '他人投资公司' ELSE '公司投资他人' END AS 类型, CASE 费用项 WHEN 321 THEN 金额 ELSE NULL 
                      END AS 投资金额, CASE 费用项 WHEN 322 THEN 金额 ELSE NULL END AS 撤资金额, A.备注, A.相关号, A.凭证号, B.简介,
                          (SELECT     dbo.Concatenate(DISTINCT 收付款方式) AS 收付款方式
                            FROM          dbo.财务_凭证收支明细 AS X
                            WHERE      (凭证 = A.凭证Id)) AS 收付款方式
FROM         dbo.视图查询_非业务费用明细 AS A INNER JOIN
                      dbo.财务_投资 AS B ON A.费用实体 = B.Id
WHERE     (A.费用实体类型 = 30)




CREATE VIEW [dbo].[视图查询_出纳票据本期]
AS
SELECT     B.日期, A.Id, CASE 支票类型 WHEN '1' THEN '现金支票' ELSE '转账支票' END AS 票据类型, A.票据号码, A.金额, A.币制, B.源
FROM         dbo.视图查询_支票 AS A INNER JOIN
                      dbo.视图查询_支票_过程结束 AS B ON A.Id = B.Id
UNION ALL
SELECT     B.日期, A.Id, '承兑汇票' AS 票据类型, A.票据号码, A.金额, 'CNY' AS 币制, CASE B.收付标志 WHEN 1 THEN '收入' ELSE '支出' END AS 源
FROM         dbo.视图查询_承兑汇票 AS A INNER JOIN
                      dbo.视图查询_承兑汇票_凭证收付 AS B ON A.Id = B.承兑汇票
UNION ALL
SELECT     B.返回时间, A.Id, '承兑汇票' AS 票据类型, A.票据号码, A.金额, 'CNY' AS 币制, CASE B.托收贴现 WHEN 1 THEN '托收' ELSE '贴现' END AS 源
FROM         dbo.视图查询_承兑汇票 AS A INNER JOIN
                      dbo.视图查询_承兑汇票_托收贴现 AS B ON A.Id = B.Id INNER JOIN
                      dbo.财务_费用实体 AS C ON C.Id = A.Id
WHERE     (C.Submitted = 1)


CREATE VIEW [dbo].[视图查询_票费用_最大时间]
AS
SELECT     费用实体, MAX(最大时间) AS 最大时间
FROM         (SELECT     费用实体, MAX(ISNULL(Updated, Created)) AS 最大时间
                       FROM          dbo.财务_费用 AS a
                       GROUP BY 费用实体
                       UNION ALL
                       SELECT     票, MAX(ISNULL(Updated, Created)) AS 最大时间
                       FROM         dbo.财务_费用信息 AS a
                       GROUP BY 票
                       UNION ALL
                       SELECT     费用实体, MAX(ISNULL(Updated, Created)) AS 最大时间
                       FROM         dbo.财务_业务费用理论值 AS a
                       GROUP BY 费用实体) AS b
GROUP BY 费用实体


CREATE VIEW dbo.视图查询_票信息_进口其他业务
AS
SELECT     A.Id, A.货代自编号, A.委托时间, A.委托人, A.提单号, A.箱量, A.标箱量, A.总重量, A.船名航次, A.船公司, A.合同号, B.受理时间, B.到港时间, B.换单时间, 
                      C.代表性箱号,
                          (SELECT     dbo.Concatenate(DISTINCT X.品名) AS Expr1
                            FROM          dbo.业务备案_普通箱 AS X INNER JOIN
                                                   dbo.视图信息_箱票 AS Y ON Y.Id = X.Id
                            WHERE      (Y.票 = A.Id)) AS 品名, B.到港时间 AS Expr1, A.件数, A.报关单号, B.进口其他业务类型, A.货物类别, A.允许应收对账, B.票箱型
FROM         dbo.业务备案_普通票 AS A INNER JOIN
                      dbo.业务备案_进口其他业务票 AS B ON A.Id = B.Id LEFT OUTER JOIN
                      dbo.视图信息_代表性箱号 AS C ON A.Id = C.票

CREATE VIEW [dbo].[视图查询_票信息_委托人]
AS
SELECT     Id, 委托时间, 委托人, 承运人, 货代自编号, 提单号, 船公司, 允许应收对账, 箱量, 11 AS 业务类型
FROM         dbo.视图查询_票信息_进口
UNION ALL
SELECT     Id, 委托时间, 委托人, 承运人, 货代自编号, 提单号, 船公司, 允许应收对账, 箱量, 15 AS 业务类型
FROM         dbo.视图查询_票信息_内贸出港
UNION ALL
SELECT     Id, 委托时间, 委托人, NULL AS 承运人, 货代自编号, 提单号, 船公司, 允许应收对账, 箱量, 45 AS 业务类型
FROM         dbo.视图查询_票信息_进口其他业务



CREATE VIEW [dbo].[视图查询_票信息_内贸出港]
AS
SELECT     A.Id, A.货代自编号, A.委托时间, A.委托人, A.提单号, A.箱量, A.标箱量, A.总重量, A.船名航次, A.船公司, A.合同号, B.承运人, B.开航日期, C.代表性箱号,
                          (SELECT     dbo.Concatenate(DISTINCT X.品名) AS Expr1
                            FROM          dbo.业务备案_普通箱 AS X INNER JOIN
                                                   dbo.视图信息_箱票 AS Y ON Y.Id = X.Id
                            WHERE      (Y.票 = A.Id)) AS 品名, B.目的港, B.进港地, B.到港时间, B.对港承运人, CASE WHEN
                          (SELECT     COUNT(id)
                            FROM          业务备案_内贸出港箱 AS X
                            WHERE      X.票 = A.Id AND 还箱时间 IS NULL) > 0 THEN '运输中' ELSE '已完成' END AS 当前状态, A.对上备注, B.预计开航日期, B.预配船名航次, 
                      B.条款, B.预计到港时间, B.预配提单号, A.货物类别, A.允许应收对账
FROM         dbo.业务备案_普通票 AS A INNER JOIN
                      dbo.业务备案_内贸出港票 AS B ON A.Id = B.Id LEFT OUTER JOIN
                      dbo.视图信息_代表性箱号 AS C ON A.Id = C.票



CREATE VIEW [dbo].[视图查询_报表值明细_票箱费用项]
AS
SELECT     a.收付标志, a.费用项, a.费用实体, a.箱, a.金额, '理论值' AS 源
FROM         dbo.财务_业务费用理论值 AS a LEFT OUTER JOIN
                      dbo.财务_费用信息 AS b ON a.费用实体 = b.票 AND a.费用项 = b.费用项
WHERE     (a.收付标志 = 2) AND (b.完全标志付 IS NULL OR
                      b.完全标志付 = 0) OR
                      (a.收付标志 = 1) AND (b.Submitted IS NULL OR
                      b.Submitted = 0)
UNION ALL
SELECT     a.收付标志, a.费用项, a.费用实体, a.箱, a.金额, '实际值' AS 源
FROM         dbo.财务_费用 AS a INNER JOIN
                      dbo.财务_费用信息 AS b ON a.费用信息 = b.Id LEFT OUTER JOIN
                      dbo.财务_对账单 AS c ON a.对账单 = c.Id
WHERE     (c.Submitted = 1) AND (a.收付标志 = 1) AND (b.Submitted = 0) OR
                      (c.Submitted = 1) AND (a.收付标志 = 2) AND (b.完全标志付 = 0) OR
                      (a.收付标志 = 1) AND (b.Submitted = 0) AND (a.凭证费用明细 IS NOT NULL) OR
                      (a.收付标志 = 2) AND (b.完全标志付 = 0) AND (a.凭证费用明细 IS NOT NULL)



CREATE VIEW [dbo].[视图查询_票信息_进口]
AS
SELECT     A.Id, A.货代自编号, A.委托时间, A.委托人, A.提单号, A.箱量, A.标箱量, A.总重量, A.船名航次, A.船公司, A.合同号, B.转关标志, B.承运人, B.放行时间, 
                      B.到港时间, B.单证齐全时间, B.结关时间, C.代表性箱号,
                          (SELECT     dbo.Concatenate(DISTINCT X.品名) AS Expr1
                            FROM          dbo.业务备案_普通箱 AS X INNER JOIN
                                                   dbo.视图信息_箱票 AS Y ON Y.Id = X.Id
                            WHERE      (Y.票 = A.Id)) AS 品名, CASE WHEN isnull(B.到港时间, getdate()) >= isnull(B.单证齐全时间, getdate()) THEN 0 ELSE DATEDIFF(day, 
                      B.到港时间, isnull(B.单证齐全时间, getdate())) END AS 单证晚到天数, CASE WHEN B.到港时间 IS NULL THEN '未到港' WHEN B.放行时间 IS NULL 
                      THEN '报关中' WHEN B.承运标志 = 1 AND
                          (SELECT     COUNT(id)
                            FROM          业务备案_进口箱 AS X
                            WHERE      X.票 = A.Id AND 还箱时间 IS NULL) > 0 THEN '运输中' ELSE '已完成' END AS 当前状态, B.操作完全标志, A.件数, A.报关单号, B.报关员, 
                      B.转关指运地, A.货物类别, B.产地, B.卸箱地, B.单证晚到, B.免箱联系货主时间, B.最终免箱标志, A.允许应收对账, A.对上备注, A.对下备注
FROM         dbo.业务备案_普通票 AS A INNER JOIN
                      dbo.业务备案_进口票 AS B ON A.Id = B.Id LEFT OUTER JOIN
                      dbo.视图信息_代表性箱号 AS C ON A.Id = C.票


CREATE VIEW [dbo].[视图查询_对账单_票]
AS
SELECT     F.收付标志, F.Id, F.费用项, F.金额, F.相关人, D.结算期限, F.对账单, D.Submitted AS 状态, D.关账日期, D.对账单类型, F.费用实体, F.箱, C.业务, 
                      D.编号 AS 对账单号, D.费用项 AS 费用类型
FROM         dbo.财务_对账单 AS D INNER JOIN
                      dbo.财务_费用 AS F ON D.Id = F.对账单 INNER JOIN
                      dbo.财务_费用实体 AS A ON F.费用实体 = A.Id INNER JOIN
                      dbo.视图信息_费用实体类型_业务 AS C ON A.费用实体类型 = C.费用实体类型


CREATE VIEW [dbo].[视图查询_出纳资金当前]
AS
SELECT     SUM(CASE 存取标志 WHEN 1 THEN 金额 ELSE 0 - 金额 END) AS 金额, NULL AS 票据数量, '银行账户:' + B.简称 AS 项目, B.币制
FROM         dbo.视图查询_银行日记帐 AS A INNER JOIN
                      dbo.参数备案_银行账户 AS B ON B.Id = A.银行账户
GROUP BY B.简称, B.币制
UNION ALL
SELECT     SUM(CASE 存取标志 WHEN 2 THEN 金额 ELSE 0 - 金额 END) AS 金额, NULL AS 票据数量, '现金' AS 项目, 币制
FROM         dbo.视图查询_现金日记帐 AS A
GROUP BY 币制
UNION ALL
SELECT     SUM(A.金额) AS 金额, COUNT(*) AS 票据数量, 
                      CASE 托收贴现 WHEN 1 THEN '承兑汇票:在途托收' WHEN 2 THEN '承兑汇票:在途贴现' ELSE '承兑汇票:未使用' END AS 项目, 'CNY' AS Expr1
FROM         dbo.视图查询_承兑汇票 AS A INNER JOIN
                      dbo.财务_费用实体 AS C ON C.Id = A.Id
WHERE     (C.Submitted = 0)
GROUP BY A.托收贴现
UNION ALL
SELECT     SUM(A.金额) AS 金额, COUNT(*) AS 票据数量, '支票:未使用' AS 项目, B.币制
FROM         dbo.财务_支票 AS A INNER JOIN
                      dbo.参数备案_银行账户 AS B ON B.Id = A.银行账户
WHERE     (A.是否作废 = 0) AND (A.Submitted = 0) AND (A.领用方式 IS NULL)
GROUP BY B.币制
UNION ALL
SELECT     SUM(A.金额) AS 金额, COUNT(*) AS 票据数量, '支票:在途' AS 项目, B.币制
FROM         dbo.财务_支票 AS A INNER JOIN
                      dbo.参数备案_银行账户 AS B ON B.Id = A.银行账户
WHERE     (A.是否作废 = 0) AND (A.Submitted = 0) AND (A.领用方式 IS NOT NULL)
GROUP BY B.币制
UNION ALL
SELECT     SUM(CASE 收付标志 WHEN 1 THEN 金额 ELSE 0 - 金额 END) AS 金额, NULL AS 票据数量, '总资金' AS 项目, 币制
FROM         dbo.视图查询_资金日记帐 AS A
GROUP BY 币制


CREATE VIEW [dbo].[视图查询_进口_常规应收对账单]
AS
SELECT     委托人, COUNT(货代自编号) AS 未对账票数
FROM         dbo.视图查询_进口_常规应收对账单_明细
WHERE     (常规费用可对账 = 'true') AND (常规费用已对账 = 'false')
GROUP BY 委托人


CREATE VIEW [dbo].[视图查询_票费用项]
AS
SELECT     B.Id AS 费用实体, A.编号 AS 费用项, A.费用类别, A.SeqNo, B.费用实体类型 AS 业务类型
FROM         dbo.视图费用项_业务 AS A CROSS JOIN
                      dbo.财务_费用实体 AS B
WHERE     (A.现有费用实体类型 LIKE '%11,%') AND (B.费用实体类型 = 11)
UNION ALL
SELECT     B.Id AS 费用实体, A.编号 AS 费用项, A.费用类别, A.SeqNo, B.费用实体类型 AS 业务类型
FROM         dbo.视图费用项_业务 AS A CROSS JOIN
                      dbo.财务_费用实体 AS B
WHERE     (A.现有费用实体类型 LIKE '%15,%') AND (B.费用实体类型 = 15)
UNION ALL
SELECT     B.Id AS 费用实体, A.编号 AS 费用项, A.费用类别, A.SeqNo, B.费用实体类型 AS 业务类型
FROM         dbo.视图费用项_业务 AS A CROSS JOIN
                      dbo.财务_费用实体 AS B
WHERE     (A.现有费用实体类型 LIKE '%45,%') AND (B.费用实体类型 = 45)


