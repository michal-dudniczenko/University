package Frontend;

import Backend.*;
import javax.swing.*;
import javax.swing.border.EmptyBorder;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

public class PrzyciskSortujOsoby {
    public JFrame frame;
    public PrzyciskSortujOsoby() {
        SwingUtilities.invokeLater(new Runnable() {
            @Override
            public void run() {
                frame = new JFrame("Sortowanie liczby osób");
                frame.add(new Panel1());
                frame.pack();
                frame.setLocationRelativeTo(null);
                frame.setDefaultCloseOperation(JFrame.DO_NOTHING_ON_CLOSE);
                frame.setResizable(false);
                frame.setVisible(true);
            }
        });
    }
    public class Panel1 extends JPanel{
        public Panel1(){
            setBorder(new EmptyBorder(10, 10, 10, 10));
            setLayout(new GridBagLayout());

            GridBagConstraints gbc = new GridBagConstraints();
            gbc.gridwidth = GridBagConstraints.REMAINDER;

            gbc.anchor = GridBagConstraints.CENTER;
            gbc.fill = GridBagConstraints.HORIZONTAL;

            JPanel elementy = new JPanel(new GridBagLayout());

            JLabel label1 = new JLabel("Wybierz rodzaj sortowania:");elementy.add(label1, gbc);
            JPanel wyborSortowania = new JPanel(new FlowLayout());
            JCheckBox checkBox1 = new JCheckBox("Po nazwisku");wyborSortowania.add(checkBox1);
            JCheckBox checkBox2 = new JCheckBox("Po nazwisku i imieniu");wyborSortowania.add(checkBox2);
            JCheckBox checkBox3 = new JCheckBox("Po nazwisku i wieku");wyborSortowania.add(checkBox3);
            checkBox1.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox2.isSelected())checkBox2.setSelected(false);
                    if (checkBox3.isSelected())checkBox3.setSelected(false);
                }
            });
            checkBox2.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox1.isSelected())checkBox1.setSelected(false);
                    if (checkBox3.isSelected())checkBox3.setSelected(false);
                }
            });
            checkBox3.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox2.isSelected())checkBox2.setSelected(false);
                    if (checkBox1.isSelected())checkBox1.setSelected(false);
                }
            });

            elementy.add(wyborSortowania, gbc);

            elementy.add(new JLabel(" "), gbc);
            JButton button0 = new JButton("Zatwierdź");
            button0.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    String wybor="1";
                    if (checkBox2.isSelected())wybor="2";
                    else if (checkBox3.isSelected())wybor="3";

                    Main.setOsoby(Funkcje.sortujOsoby(Main.getOsoby(), wybor));
                    new PrzyciskKontynuuj("Pomyślnie posortowano listę osób.");
                    frame.dispose();
                }
            });
            elementy.add(button0, gbc);

            JButton button0_1 = new JButton("Wróć");
            button0_1.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    frame.dispose();
                }
            });
            elementy.add(button0_1, gbc);

            gbc.weighty = 1;
            add(elementy, gbc);

        }
    }

}
