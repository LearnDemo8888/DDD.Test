using Microsoft.EntityFrameworkCore;


namespace EFCore领域事件发布的时机;
using MediatR;

public class UserDbContext:DbContext
{

    private readonly IMediator _mediator;

    public UserDbContext(DbContextOptions<UserDbContext> options, IMediator mediator) : base(options)
    {

        _mediator = mediator;


    }


    public DbSet<User> Users { get; private set; }



    public override  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {

        //得到所用在跟踪中的实体继承IDomainEvents接口，含有待发送的消息对象
        var domainEntities = this.ChangeTracker.Entries<IDomainEvents>().Where(x=>x.Entity.GetDomainEvents().Any());

        //得到所有待发布消息
        var domainEvents = domainEntities.SelectMany(x => x.Entity.GetDomainEvents()).ToList();

        //清除所有待发布
        domainEntities.ToList().ForEach(x => x.Entity.ClearDomainEvents());


        //发布所有消息
 
        domainEvents.ForEach(async e => await this._mediator.Publish(e));

        //把消息发布放到base.SaveChangesAsync(()之前，可以保存领域事件响应代码中的
        //事件操作和base.SaveChangesAsync中的代码在同一个事务中，实现强一致性
        return  base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }
}