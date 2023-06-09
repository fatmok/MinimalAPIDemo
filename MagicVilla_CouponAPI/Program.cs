using AutoMapper;
using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/api/coupon", (ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "Getting all Coupons");
    return Results.Ok(CouponStore.couponList);

}).WithName("GetCoupons").Produces<IEnumerable<Coupon>>(200);


app.MapGet("/api/coupon/{id:int}", (int id) =>
{
    return Results.Ok(CouponStore.couponList.FirstOrDefault(u => u.Id == id));

}).WithName("GetCoupon").Produces<Coupon>(200);

app.MapPost("/api/coupon", (IMapper _mapper,[FromBody] CouponCreateDTO coupon_C_DTO) =>
{

    if (string.IsNullOrEmpty(coupon_C_DTO.Name))
    {
        return Results.BadRequest("Invalid Id or Coupon Name");
    }

    if (CouponStore.couponList.FirstOrDefault(u => u.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null)
    {
        return Results.BadRequest("Coupon Name already Exists");
    }
    Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);
   
    coupon.Id = CouponStore.couponList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
    CouponStore.couponList.Add(coupon);

    CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

    return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, coupon);
    // return Results.Created($"/api/coupon/{coupon.Id}",coupon);



}).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<CouponDTO>(201).Produces(400);


app.MapPut("/api/coupon", () =>
{


});

app.MapDelete("/api/coupon/{id:int}", (int id) =>
{


});


app.UseHttpsRedirection();


app.Run();

