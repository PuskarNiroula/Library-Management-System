namespace Library_Management_System.Services.Admin.BookRequest;

public interface IBookRequestService
{
    
    public Task Reject(int id); 
    public Task Approve(int id,int userId);
    
}