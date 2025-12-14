using CoolLibrary.Domain.Entities;
using CoolLibrary.Domain.Enums;
using HotChocolate.Types;

namespace CoolLibrary.Application.GraphQL.Types;

/// <summary>
/// Custom GraphQL type for Customer entity
/// Adds computed fields and hides sensitive data
/// </summary>
public class CustomerType : ObjectType<Customer>
{
    protected override void Configure(IObjectTypeDescriptor<Customer> descriptor)
    {
        descriptor.Description("Library customer with extended computed fields");
        
        // Campo calculado: Total de multas pendientes
        descriptor
            .Field("totalOutstandingFines")
            .Description("Total amount of unpaid fines")
            .Type<DecimalType>()
            .Resolve(ctx => 
            {
                var customer = ctx.Parent<Customer>();
                return customer.Fines?
                    .Where(f => f.Status == FineStatus.Unpaid)
                    .Sum(f => f.Amount) ?? 0m;
            });
        
        // Campo calculado: Préstamos vencidos
        descriptor
            .Field("overdueLoansCount")
            .Description("Number of currently overdue loans")
            .Type<IntType>()
            .Resolve(ctx =>
            {
                var customer = ctx.Parent<Customer>();
                return customer.Loans?
                    .Count(l => l.Status == LoanStatus.Overdue) ?? 0;
            });
        
        // Campo calculado: Puede pedir más libros
        descriptor
            .Field("canRequestMoreBooks")
            .Description("Indicates if customer can request more books")
            .Type<BooleanType>()
            .Resolve(ctx =>
            {
                var customer = ctx.Parent<Customer>();
                var activeLoans = customer.Loans?.Count(l => l.Status == LoanStatus.Active) ?? 0;
                var hasOverdueFines = customer.Fines?.Any(f => f.Status == FineStatus.Unpaid) ?? false;
                
                return !hasOverdueFines && 
                       activeLoans < customer.MaxBooksAllowed && 
                       customer.MembershipStatus == MembershipStatus.Active;
            });
    }
}
