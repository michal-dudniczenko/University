package ztw.library_rest_api.services;

import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import ztw.library_rest_api.models.Author;
import ztw.library_rest_api.models.Book;
import ztw.library_rest_api.models.Response;

import java.util.ArrayList;
import java.util.List;

@Service
public class AuthorsService implements IAuthorsService {
    private final static List<Author> authorsRepo = new ArrayList<>();
    private static int currentId = 1;

    static {
        authorsRepo.add(new Author(currentId++, "Henryk", "Sienkiewicz"));
        authorsRepo.add(new Author(currentId++, "Stanisław", "Reymont"));
        authorsRepo.add(new Author(currentId++, "Adam", "Mickiewicz"));
        authorsRepo.add(new Author(currentId++, "Juliusz", "Słowacki"));
        authorsRepo.add(new Author(currentId++, "Bolesław", "Prus"));
        authorsRepo.add(new Author(currentId++, "Zofia", "Nałkowska"));
        authorsRepo.add(new Author(currentId++, "Bruno", "Schulz"));
        authorsRepo.add(new Author(currentId++, "Czesław", "Miłosz"));
        authorsRepo.add(new Author(currentId++, "Wisława", "Szymborska"));
        authorsRepo.add(new Author(currentId++, "Olga", "Tokarczuk"));
    }

    @Override
    public Response getAuthors(int idFrom, int idTo) {
        if (idFrom >= authorsRepo.size()) {
            return new Response(HttpStatus.BAD_REQUEST, "Starting index out of bounds.");
        }

        return new Response(HttpStatus.OK, authorsRepo.subList(idFrom, Math.min(idTo + 1, authorsRepo.size())));
    }

    @Override
    public Response getAuthor(int id) {
        for (Author a : authorsRepo) {
            if (a.getId() == id) {
                return new Response(HttpStatus.OK, a);
            }
        }
        return new Response(HttpStatus.NOT_FOUND);
    }

    @Override
    public Response getAuthorsCount() {
        return new Response(HttpStatus.OK, authorsRepo.size());
    }

    @Override
    public Response addAuthor(Author newAuthor) {
        newAuthor.setId(currentId);
        currentId++;
        authorsRepo.add(newAuthor);
        return new Response(HttpStatus.OK, newAuthor);
    }

    @Override
    public Response updateAuthor(Author author) {
        for (int i = 0; i < authorsRepo.size(); i++) {
            if (authorsRepo.get(i).getId() == author.getId()) {
                authorsRepo.set(i, author);
                return new Response(HttpStatus.OK);
            }
        }
        return new Response(HttpStatus.NOT_FOUND, "No author with specified id in repository.");
    }

    @Override
    public Response deleteAuthor(int id, IBooksService booksService) {
        for (int i = 0; i < authorsRepo.size(); i++) {
            if (authorsRepo.get(i).getId() == id) {
                for (Book book : booksService.getBooksFromAuthor(id)) {
                        book.setAuthorId(-1);
                }
                authorsRepo.remove(i);
                return new Response(HttpStatus.OK);
            }
        }
        return new Response(HttpStatus.NOT_FOUND, "No author with specified id in repository.");
    }
}
