namespace Library_Management_System.Services.Admin.Exception;

public class BookRequestNotFoundException(int id):System.Exception("Book request with ID: " + id + " not found");