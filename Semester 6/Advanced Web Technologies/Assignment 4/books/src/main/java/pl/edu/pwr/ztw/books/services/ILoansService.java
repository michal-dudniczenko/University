package pl.edu.pwr.ztw.books.services;

import pl.edu.pwr.ztw.books.models.Book;
import pl.edu.pwr.ztw.books.models.Loan;

import java.util.Collection;

public interface ILoansService {
    Collection<Loan> getLoans();

    Loan getLoan(int id);

    boolean borrowBook(int readerId, int bookId, IBooksService booksService, IReadersService readersService);

    boolean returnBook(int readerId, int bookId);

    int getBorrowedBooksCount(int readerId);

    Collection<Book> getBorrowedBooks(int readerId, IBooksService booksService);
}
