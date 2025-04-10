package ztw.library_rest_api.services;

import ztw.library_rest_api.models.Book;
import ztw.library_rest_api.models.Response;

import java.util.Collection;

public interface IBooksService {

    Response getBooks(int idFrom, int idTo);

    Response getBook(int id);

    Response getBooksCount();

    Response addBook(Book newBook, IAuthorsService authorsService);

    Response updateBook(Book book, IAuthorsService authorsService);

    Response deleteBook(int id, ILoansService loansService);

    Collection<Book> getBooksFromAuthor(int authorId);
}
