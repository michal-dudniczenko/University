package pl.edu.pwr.ztw.books.services;

import pl.edu.pwr.ztw.books.models.Book;

import java.util.Collection;

public interface IBooksService {

    Collection<Book> getBooks();

    Book getBook(int id);

    boolean addBook(Book newBook, IAuthorsService authorsService);

    boolean updateBook(Book book, IAuthorsService authorsService);

    boolean deleteBook(int id);

    Collection<Book> getBooksFromAuthor(int authorId);
}
