package pl.edu.pwr.ztw.books.services;

import pl.edu.pwr.ztw.books.models.Author;

import java.util.Collection;

public interface IAuthorsService {

    Collection<Author> getAuthors();

    Author getAuthor(int id);

    void addAuthor(Author newAuthor);

    boolean updateAuthor(Author author);

    boolean deleteAuthor(int id, IBooksService booksService);
}
