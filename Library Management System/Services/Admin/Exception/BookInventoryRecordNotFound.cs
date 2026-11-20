namespace Library_Management_System.Services.Admin.Exception;

public class BookInventoryRecordNotFound(int id) : System.Exception("Book with ID: " + id + " has no inventory record.");