package ztw.library_rest_api.services;

import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import ztw.library_rest_api.models.Book;
import ztw.library_rest_api.models.Response;

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
    public Response getBooks(int idFrom, int idTo) {
        if (idFrom >= booksRepo.size()) {
            return new Response(HttpStatus.BAD_REQUEST, "Starting index out of bounds.");
        }

        return new Response(HttpStatus.OK, booksRepo.subList(idFrom, Math.min(idTo + 1, booksRepo.size())));
    }

    @Override
    public Response getBook(int id) {
        for (Book b : booksRepo) {
            if (b.getId() == id) {
                return new Response(HttpStatus.OK, b);
            }
        }
        return new Response(HttpStatus.NOT_FOUND);
    }

    @Override
    public Response addBook(Book newBook, IAuthorsService authorsService) {
        if (authorsService.getAuthor(newBook.getAuthorId()).getPayload() == null) {
            return new Response(HttpStatus.BAD_REQUEST, "No author with specified id in repository.");
        }

        newBook.setId(currentId);
        currentId++;
        booksRepo.add(newBook);
        return new Response(HttpStatus.CREATED, newBook);
    }

    @Override
    public Response updateBook(Book book, IAuthorsService authorsService) {
        if (authorsService.getAuthor(book.getAuthorId()).getPayload() == null) {
            return new Response(HttpStatus.BAD_REQUEST, "No author with specified id in repository.");
        }

        for (int i = 0; i < booksRepo.size(); i++) {
            if (booksRepo.get(i).getId() == book.getId()) {
                booksRepo.set(i, book);
                return new Response(HttpStatus.OK);
            }
        }
        return new Response(HttpStatus.NOT_FOUND, "No book with specified id in repository.");
    }

    @Override
    public Response deleteBook(int id, ILoansService loansService) {
        for (int i = 0; i < booksRepo.size(); i++) {
            if (booksRepo.get(i).getId() == id) {
                loansService.bookDeletionClear(id);
                booksRepo.remove(i);
                return new Response(HttpStatus.OK);
            }
        }
        return new Response(HttpStatus.NOT_FOUND, "No book with specified id in repository.");
    }

    @Override
    public Collection<Book> getBooksFromAuthor(int authorId) {
        return booksRepo.stream()
                .filter(book -> book.getAuthorId() == authorId)
                .toList();
    }
}
