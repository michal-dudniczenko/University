package pl.edu.pwr.ztw.books.services;

import org.springframework.stereotype.Service;
import pl.edu.pwr.ztw.books.models.Book;
import pl.edu.pwr.ztw.books.models.Loan;
import pl.edu.pwr.ztw.books.models.Reader;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

@Service
public class LoansService implements ILoansService{
    private final static List<Loan> loansRepo = new ArrayList<>();
    private static int currentId = 1;

    private final static int maxBorrowedBooks = 3;
    
    static {
        loansRepo.add(new Loan(1, 1, 2));
        currentId++;
        loansRepo.add(new Loan(2, 2, 3));
        currentId++;
        loansRepo.add(new Loan(3,1, 1));
        currentId++;
    }

    @Override
    public Collection<Loan> getLoans() {
        return loansRepo;
    }

    @Override
    public Loan getLoan(int id) {
        return loansRepo.stream()
                .filter(loan -> loan.getId() == id)
                .findAny()
                .orElse(null);
    }

    @Override
    public boolean borrowBook(int readerId, int bookId, IBooksService booksService, IReadersService readersService) {
        if (booksService.getBook(bookId) == null || readersService.getReader(readerId) == null ||
                readersService.getReader(readerId).isActive() == false || getBorrowedBooksCount(readerId) >= maxBorrowedBooks) {
            return false;
        }

        loansRepo.add(new Loan(currentId, readerId, bookId));
        currentId++;
        return true;
    }

    @Override
    public boolean returnBook(int readerId, int bookId) {
        for (Loan loan : loansRepo) {
            if (loan.getReaderId() == readerId && loan.getBookId() == bookId && loan.isWasReturned() == false) {
                loan.setWasReturned(true);
                return true;
            }
        }
        return false;
    }

    @Override
    public int getBorrowedBooksCount(int readerId) {
        int booksCount = 0;
        for (Loan loan : loansRepo) {
            if (loan.getReaderId() == readerId && loan.isWasReturned() == false) {
                booksCount++;
            }
        }
        return booksCount;
    }

    @Override
    public Collection<Book> getBorrowedBooks(int readerId, IBooksService booksService) {
        return List.of();
    }
}
