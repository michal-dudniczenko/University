package ztw.library_rest_api.services;

import ztw.library_rest_api.models.Response;

public interface ILoansService {

    Response getLoans(int idFrom, int idTo);

    Response getLoan(int id);

    Response getLoansCount();

    Response getReaderActiveLoans(int readerId, IReadersService readersService);

    Response borrowBook(int readerId, int bookId, IBooksService booksService, IReadersService readersService);

    Response returnBook(int readerId, int bookId);

    int getBorrowedBooksCount(int readerId);

    boolean isBookBorrowed(int bookId);

    void bookDeletionClear(int bookId);

    void readerDeletionClear(int readerId);
}
