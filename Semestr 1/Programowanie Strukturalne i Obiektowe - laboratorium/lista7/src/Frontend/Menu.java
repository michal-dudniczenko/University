package Frontend;

import Backend.*;
import javax.swing.*;
import javax.swing.border.EmptyBorder;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectOutputStream;

public class Menu {
    public JFrame frame;
    public Menu() {
        SwingUtilities.invokeLater(new Runnable() {
            @Override
            public void run() {
                frame = new JFrame("Menu");
                frame.add(new PrzyciskiMenu());
                frame.pack();
                frame.setLocationRelativeTo(null);
                frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
                frame.setResizable(false);
                frame.setVisible(true);
            }
        });
    }
    public class PrzyciskiMenu extends JPanel{
        public PrzyciskiMenu(){
            setBorder(new EmptyBorder(10, 10, 10, 10));
            setLayout(new GridBagLayout());

            GridBagConstraints gbc = new GridBagConstraints();
            gbc.gridwidth = GridBagConstraints.REMAINDER;
            gbc.anchor = GridBagConstraints.NORTH;
            JLabel naglowek = new JLabel("MENU");
            naglowek.setFont(new Font("a", Font.PLAIN, 30));
            naglowek.setForeground(new Color(88, 38, 189));
            add(naglowek, gbc);
            add(new JLabel(" "), gbc);

            gbc.anchor = GridBagConstraints.CENTER;
            gbc.fill = GridBagConstraints.HORIZONTAL;

            JPanel buttons = new JPanel(new GridBagLayout());
            JButton button1 = new JButton("Dodaj studenta");buttons.add(button1, gbc);
            button1.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskDodajStudenta();
                }
            });
            JButton button1_1 = new JButton("Dodaj pracownika administracji");buttons.add(button1_1, gbc);
            button1_1.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskDodajPracownikaAdministracji();
                }
            });
            JButton button1_2 = new JButton("Dodaj pracownika badawczo-dydaktycznego");buttons.add(button1_2, gbc);
            button1_2.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskDodajPracownikaBD();
                }
            });
            JButton button2 = new JButton("Dodaj kurs");buttons.add(button2, gbc);
            button2.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskDodajKurs();
                }
            });
            JButton button3 = new JButton("Wyszukaj pracownika");buttons.add(button3, gbc);
            button3.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskWyszukajPracownikow();
                }
            });
            JButton button4 = new JButton("Wyszukaj studenta");buttons.add(button4, gbc);
            button4.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskWyszukajStudentow();
                }
            });
            JButton button5 = new JButton("Wyszukaj kurs");buttons.add(button5, gbc);
            button5.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskWyszukajKursy();
                }
            });
            JButton button6 = new JButton("Wyświetl wszystkich pracowników");buttons.add(button6, gbc);
            button6.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskWyswietlPracownikow();
                }
            });
            JButton button7 = new JButton("Wyświetl wszystkich studentów");buttons.add(button7, gbc);
            button7.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskWyswietlStudentow();
                }
            });
            JButton button8 = new JButton("Wyświetl wszystkie kursy");buttons.add(button8, gbc);
            button8.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskWyswietlKursy();
                }
            });
            JButton button9 = new JButton("Sortuj listę osób");buttons.add(button9, gbc);
            button9.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskSortujOsoby();
                }
            });
            JButton button10 = new JButton("Sortuj listę kursów");buttons.add(button10, gbc);
            button10.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    Main.setKursy(Funkcje.sortujKursy(Main.getKursy()));
                    new PrzyciskKontynuuj("Pomyślnie posortowano listę kursów.");
                }
            });
            JButton button11 = new JButton("Usuń pracowników");buttons.add(button11, gbc);
            button11.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskUsunPracownika();
                }
            });
            JButton button12 = new JButton("Usuń studentów");buttons.add(button12, gbc);
            button12.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskUsunStudenta();
                }
            });
            JButton button13 = new JButton("Usuń kursy");buttons.add(button13, gbc);
            button13.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskUsunKurs();
                }
            });
            JButton button14 = new JButton("Wyczyść listę studentów");buttons.add(button14, gbc);
            button14.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskCzyszczenie();
                }
            });
            JButton button15 = new JButton("Dodaj ogłoszenie");buttons.add(button15, gbc);
            button15.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    new PrzyciskOgloszenie();
                }
            });
            JButton button16 = new JButton("Usuń duplikaty");buttons.add(button16, gbc);
            button16.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    Main.setOsoby(Funkcje.usunDuplikaty(Main.getOsoby()));
                    new PrzyciskKontynuuj("Pomyślnie usunięto duplikaty z listy osób.");
                }
            });
            JButton button0 = new JButton("Zakończ i zapisz");buttons.add(button0, gbc);
            //serializacja i zamknięcie
            button0.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    try{
                        ObjectOutputStream oos = new ObjectOutputStream(new FileOutputStream("osoby.txt"));
                        oos.writeObject(Main.getOsoby());
                        oos = new ObjectOutputStream(new FileOutputStream("kursy.txt"));
                        oos.writeObject(Main.getKursy());
                        oos.close();
                    }catch(IOException ioe){
                        ioe.printStackTrace();
                    }
                    frame.dispose();
                }
            });

            gbc.weighty = 1;
            add(buttons, gbc);
        }
    }
}
