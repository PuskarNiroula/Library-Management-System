namespace Library_Management_System.Services.Student.Exception;

public class DuplicateBookRequestException(
    string message = "You have already rented this book or submitted a request for it."
) : System.Exception(message);