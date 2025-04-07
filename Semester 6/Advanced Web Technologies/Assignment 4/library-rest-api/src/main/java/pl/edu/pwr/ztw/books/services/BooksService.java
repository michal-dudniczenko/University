package pl.edu.pwr.ztw.books.services;

import org.springframework.stereotype.Service;
import pl.edu.pwr.ztw.books.models.Book;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

@Service
public class BooksService implements IBooksService {
    private final static List<Book> booksRepo = new ArrayList<>();
    private static int currentId = 1;

    static {
        booksRepo.add(new Book(1, "Potop", 1, 936));
        currentId++;
        booksRepo.add(new Book(2, "Wesele", 2, 150));
        currentId++;
        booksRepo.add(new Book(3, "Dziady", 3, 292));
        currentId++;
    }

    @Override
    public Collection<Book> getBooks() {
        return booksRepo;
    }

    @Override
    public Book getBook(int id) {
        return booksRepo.stream()
                .filter(book -> book.getId() == id)
                .findAny()
                .orElse(null);
    }

    @Override
    public boolean addBook(Book newBook, IAuthorsService authorsService) {
        if (authorsService.getAuthor(newBook.getAuthorId()) == null) {
            return false;
        }

        newBook.setId(currentId);
        currentId++;
        booksRepo.add(newBook);
        return true;
    }

    @Override
    public boolean updateBook(Book book, IAuthorsService authorsService) {
        if (authorsService.getAuthor(book.getAuthorId()) == null) {
            return false;
        }

        for (int i = 0; i < booksRepo.size(); i++) {
            if (booksRepo.get(i).getId() == book.getId()) {
                booksRepo.set(i, book);
                return true;
            }
        }
        return false;
    }

    @Override
    public boolean deleteBook(int id) {
        for (int i = 0; i < booksRepo.size(); i++) {
            if (booksRepo.get(i).getId() == id) {
                booksRepo.remove(i);
                return true;
            }
        }
        return false;
    }

    @Override
    public Collection<Book> getBooksFromAuthor(int authorId) {
        return booksRepo.stream()
                .filter(book -> book.getAuthorId() == authorId)
                .toList();
    }
}
