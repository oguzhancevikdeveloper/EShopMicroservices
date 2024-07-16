﻿using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services;

public class DiscountService(DiscountContext discountContext, ILogger<DiscountService> logger) : DiscountProtoService.DiscountProtoServiceBase
{
    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>();

        if (coupon is null) throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object"));

        discountContext.Coupons.Add(coupon);
        await discountContext.SaveChangesAsync();

        logger.LogInformation("Discount is successfully created. ProductName :{ProductName}", coupon.ProductName);

        var couponModel = coupon.Adapt<CouponModel>();

        return couponModel;
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {

        var deletedCoupon = await discountContext.Coupons.FirstOrDefaultAsync(x => x.ProductName.Equals(request.ProductName));

        if (deletedCoupon is null) throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} is not found."));

        discountContext.Coupons.Remove(deletedCoupon);
        await discountContext.SaveChangesAsync();

        logger.LogInformation("Discount is successfully deleted. ProductName : {ProductName}", request.ProductName);
        return new DeleteDiscountResponse { Success = true };
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var updateCoupon = request.Coupon.Adapt<Coupon>();

        if (updateCoupon is null) throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));

        discountContext.Coupons.Update(updateCoupon);
        await discountContext.SaveChangesAsync();
        logger.LogInformation("Discount is successfully updated. ProductName : {ProductName}", updateCoupon.ProductName);

        var coupon = updateCoupon.Adapt<CouponModel>();

        return coupon;
    }

    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var coupon = await discountContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

        if (coupon is null) coupon = new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };

        logger.LogInformation("Discount is retrieved for ProductName : {productName}, Amount : {amount}", coupon.ProductName, coupon.Amount);

        var couponModel = coupon.Adapt<CouponModel>();
        return couponModel;
    }
}