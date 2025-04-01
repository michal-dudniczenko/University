package Frontend;

import Backend.*;
import javax.swing.*;
import javax.swing.border.EmptyBorder;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.ArrayList;

public class PrzyciskKursyStudenta {
    public JFrame frame;
    public ArrayList<Kursy> kursy = Main.getKursy();
    public ArrayList<Kursy> kursyTemp = new ArrayList<Kursy>();

    public ArrayList<Kursy> getKursyTemp(){
        return kursyTemp;
    }

    public PrzyciskKursyStudenta() {
        SwingUtilities.invokeLater(new Runnable() {
            @Override
            public void run() {
                frame = new JFrame("Wybór kursów");
                frame.add(new Panel1());
                frame.pack();
                frame.setLocationRelativeTo(null);
                frame.setDefaultCloseOperation(JFrame.DO_NOTHING_ON_CLOSE);
                frame.setResizable(false);
                frame.setVisible(true);
            }
        });
    }

    public class Panel1 extends JPanel {
        public Panel1() {
            setBorder(new EmptyBorder(10, 10, 10, 10));
            setLayout(new GridBagLayout());

            GridBagConstraints gbc = new GridBagConstraints();
            gbc.gridwidth = GridBagConstraints.REMAINDER;
            gbc.anchor = GridBagConstraints.NORTH;
            JLabel naglowek = new JLabel("Wybierz kursy studenta:");
            naglowek.setFont(new Font("a", Font.BOLD, 15));
            add(naglowek, gbc);

            gbc.anchor = GridBagConstraints.CENTER;
            gbc.fill = GridBagConstraints.HORIZONTAL;

            JPanel elementy = new JPanel(new GridBagLayout());
            ArrayList<JCheckBox> checkBoxes = new ArrayList<JCheckBox>();
            for (int i = 0; i< Main.getKursy().size(); i++) {
                JCheckBox temp = new JCheckBox((i + 1) + ". " + Main.getKursy().get(i).toString());
                checkBoxes.add(temp);
                elementy.add(temp, gbc);
            }

            elementy.add(new JLabel(" "), gbc);
            JButton button0 = new JButton("Zatwierdź");
            button0.addActionListener(new ActionListener() {
                public void actionPerformed(ActionEvent e) {
                    for (int i=0; i<checkBoxes.size(); i++){
                        if (checkBoxes.get(i).isSelected()){
                            kursyTemp.add(kursy.get(i));
                        }
                    }
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
