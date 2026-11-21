using Library_Management_System.DTOs.Checkout;

namespace Library_Management_System.Services.Admin.BookRequest;

public interface IBookRequestService
{
    
    public Task Reject(int id); 
    public Task Approve(int id,int userId);
    public Task CheckoutAsync(CheckoutDto dto, int approvedBy);
    
}