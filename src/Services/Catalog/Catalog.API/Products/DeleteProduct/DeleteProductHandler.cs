using BuildingBlocks.CQRS;
using Catalog.API.Models;
using FluentValidation;
using Marten;

namespace Catalog.API.Products.DeleteProduct;

public record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResult>;

public record DeleteProductResult(bool IsSuccess);

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Product ID is required");
    }
}

internal class DeleteProductCommandHandler(IDocumentSession documentSession) : ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
    public async Task<DeleteProductResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        documentSession.Delete<Product>(request.Id);
        await documentSession.SaveChangesAsync(cancellationToken);

        return new DeleteProductResult(true);
    }
}
