using AutoPay.API.DB;
using AutoPay.Common.DTOs.Requests;
using AutoPay.Common.DTOs.Responses;
using AutoPay.Common.DTOs.Responses.Errors;
using AutoPay.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoPay.API.Controllers;

[ApiController]
[Route("api/checks")]
[Authorize]
public class ChecksController : ControllerBase
{
    private readonly DataContext _dataContext;

    public ChecksController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CheckResponse>>> GetChecks()
    {
        var checks = await _dataContext.SettlementChecks.Include(x => x.Issuer).Include(x => x.Lines).ThenInclude(x => x.Product).ThenInclude(x => x.Image).ToListAsync();
        var checksResponse = checks.Select(x => new CheckResponse
        {
            Id = x.Id,
            Issuer = new UserResponse
            {
                Id = x.Issuer.Id,
                FullName = x.Issuer.FullName,
                PasswordHash = x.Issuer.PasswordHash,
                UserName = x.Issuer.UserName
            },
            Lines = x.Lines.Select(p => new LineResponse
            {
                Count = p.Count,
                Product = new ProductResponse
                {
                    Id = p.Product.Id,
                    Name = p.Product.Name,
                    Cost = p.Product.Cost,
                    Unit = p.Product.Unit,
                    ImageId = p.Product.Image.Id,
                    ProductId = p.Product.ProductId,
                },
                Id = p.Id
            }).ToList(),
            IssueDate = x.IssueDate,
            PaymentAmount = x.PaymentAmount,
            ActualPaymentAmount = x.ActualPaymentAmount
        });

        return new JsonResult(checksResponse);
    }

    [HttpPost("add")]
    public async Task<ActionResult<CheckResponse>> CreateCheck([FromBody] CheckRequest checkRequest)
    {
        var issuer = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == checkRequest.Issuer);
        if (issuer is null)
        {
            return BadRequest(new ErrorResponse(InternalErrorCode.NotFound));
        }


        List<CheckLine> lines = new List<CheckLine>();
        foreach (var line in checkRequest.Lines)
        {
            var checkLine = new CheckLine();

            checkLine.Id = Guid.NewGuid();
            checkLine.Count = line.Count;

            var product = await _dataContext.Products.FirstOrDefaultAsync(x => x.Id == line.ProductId);
            if (product is null)
            {
                return BadRequest(new ErrorResponse(InternalErrorCode.NotFound));
            }

            checkLine.Product = product;
            lines.Add(checkLine);
        }

        var payment = lines.Sum(x => x.Product.Cost * (decimal) x.Count);
        var check = new SettlementCheck
        {
            Id = Guid.NewGuid(),
            Issuer = issuer,
            Lines = lines,
            IssueDate = DateTime.Now,
            PaymentAmount = payment,
            ActualPaymentAmount = checkRequest.ActualPaymentAmount
        };

        _dataContext.SettlementChecks.Add(check);
        await _dataContext.SaveChangesAsync();

        return new CheckResponse
        {
            Id = check.Id,
            Issuer = new UserResponse
            {
                Id = check.Issuer.Id,
                FullName = check.Issuer.FullName,
                PasswordHash = check.Issuer.PasswordHash,
                UserName = check.Issuer.UserName
            },
            IssueDate = check.IssueDate,
            PaymentAmount = check.PaymentAmount,
            ActualPaymentAmount = check.ActualPaymentAmount,
            Lines = lines.Select(x => new LineResponse
            {
                Count = x.Count,
                Id = x.Id,
                Product = new ProductResponse
                {
                    Id = x.Product.Id,
                    Name = x.Product.Name,
                    Cost = x.Product.Cost,
                    Unit = x.Product.Unit,
                    ProductId = x.Product.ProductId,
                },
            }).ToList()
        };
    }
}