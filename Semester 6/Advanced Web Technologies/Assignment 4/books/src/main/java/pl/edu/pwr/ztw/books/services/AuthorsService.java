package pl.edu.pwr.ztw.books.services;

import org.springframework.stereotype.Service;
import pl.edu.pwr.ztw.books.models.Author;
import pl.edu.pwr.ztw.books.models.Book;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

@Service
public class AuthorsService implements IAuthorsService {
    private final static List<Author> authorsRepo = new ArrayList<>();
    private static int currentId = 1;

    static {
        authorsRepo.add(new Author(1, "Henryk", "Sienkiewicz"));
        currentId++;
        authorsRepo.add(new Author(2, "Stanisław", "Reymont"));
        currentId++;
        authorsRepo.add(new Author(3,"Adam", "Mickiewicz"));
        currentId++;
    }

    @Override
    public Collection<Author> getAuthors() {
        return authorsRepo;
    }

    @Override
    public Author getAuthor(int id) {
        return authorsRepo.stream()
                .filter(author -> author.getId() == id)
                .findAny()
                .orElse(null);
    }

    @Override
    public void addAuthor(Author newAuthor) {
        newAuthor.setId(currentId);
        currentId++;
        authorsRepo.add(newAuthor);
    }

    @Override
    public boolean updateAuthor(Author author) {
        for (int i = 0; i < authorsRepo.size(); i++) {
            if (authorsRepo.get(i).getId() == author.getId()) {
                authorsRepo.set(i, author);
                return true;
            }
        }
        return false;
    }

    @Override
    public boolean deleteAuthor(int id, IBooksService booksService) {
        for (int i = 0; i < authorsRepo.size(); i++) {
            if (authorsRepo.get(i).getId() == id) {
                authorsRepo.remove(i);
                for (Book book : booksService.getBooksFromAuthor(id)) {
                    book.setAuthorId(0);
                }
                return true;
            }
        }
        return false;
    }
}
