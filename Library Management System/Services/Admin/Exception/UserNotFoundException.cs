namespace Library_Management_System.Services.Admin.Exception;

public class UserNotFoundException(int id = 0) : System.Exception("User with ID: " + id + " not found.");