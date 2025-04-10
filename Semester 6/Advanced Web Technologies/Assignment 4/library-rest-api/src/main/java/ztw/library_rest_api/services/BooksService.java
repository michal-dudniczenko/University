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
        booksRepo.add(new Book(currentId++, "Quo Vadis", 1, 500));
        booksRepo.add(new Book(currentId++, "The Knights of the Cross", 1, 650));
        booksRepo.add(new Book(currentId++, "The Peasants", 2, 800));
        booksRepo.add(new Book(currentId++, "Pan Tadeusz", 3, 400));
        booksRepo.add(new Book(currentId++, "Ballads and Romances", 3, 200));
        booksRepo.add(new Book(currentId++, "Kordian", 4, 350));
        booksRepo.add(new Book(currentId++, "Lalka", 5, 700));
        booksRepo.add(new Book(currentId++, "The Doll", 5, 720));
        booksRepo.add(new Book(currentId++, "Medallions", 6, 180));
        booksRepo.add(new Book(currentId++, "The Street of Crocodiles", 7, 250));
        booksRepo.add(new Book(currentId++, "The Captive Mind", 8, 300));
        booksRepo.add(new Book(currentId++, "The Issa Valley", 8, 280));
        booksRepo.add(new Book(currentId++, "View with a Grain of Sand", 9, 230));
        booksRepo.add(new Book(currentId++, "Here", 9, 210));
        booksRepo.add(new Book(currentId++, "Flights", 10, 450));
        booksRepo.add(new Book(currentId++, "The Books of Jacob", 10, 900));
        booksRepo.add(new Book(currentId++, "Primeval and Other Times", 10, 370));
        booksRepo.add(new Book(currentId++, "House of Day, House of Night", 10, 400));
        booksRepo.add(new Book(currentId++, "Drive Your Plow Over the Bones of the Dead", 10, 320));
        booksRepo.add(new Book(currentId++, "The Tender Narrator", 10, 280));
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
    public Response getBooksCount() {
        return new Response(HttpStatus.OK, booksRepo.size());
    }

    @Override
    public Response addBook(Book newBook, IAuthorsService authorsService) {
        if (authorsService.getAuthor(newBook.getAuthorId()).getPayload() == null) {
            return new Response(HttpStatus.BAD_REQUEST, "No author with specified id in repository.");
        }

        newBook.setId(currentId);
        currentId++;
        booksRepo.add(newBook);
        return new Response(HttpStatus.OK, newBook);
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
