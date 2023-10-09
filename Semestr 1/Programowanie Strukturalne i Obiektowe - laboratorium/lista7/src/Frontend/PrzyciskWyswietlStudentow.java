package Frontend;

import Backend.*;
import javax.swing.*;
import javax.swing.border.EmptyBorder;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

public class PrzyciskWyswietlStudentow {
    public JFrame frame;
    public PrzyciskWyswietlStudentow() {
        SwingUtilities.invokeLater(new Runnable() {
            @Override
            public void run() {
                frame = new JFrame("Wszyscy studenci");
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
            JLabel naglowek = new JLabel("Wszyscy studenci:");
            naglowek.setFont(new Font("a", Font.BOLD, 15));
            add(naglowek, gbc);
            add(new JLabel(" "),gbc );

            gbc.anchor = GridBagConstraints.CENTER;
            gbc.fill = GridBagConstraints.HORIZONTAL;

            JPanel elementy = new JPanel(new GridBagLayout());
            int licznik = 1;
            for (int i = 0; i < Main.getOsoby().size(); i++) {
                if (Main.getOsoby().get(i) instanceof Student) {
                    elementy.add(new JLabel((licznik) + ". " + Main.getOsoby().get(i).toString()), gbc);
                    licznik++;
                    String kursyTemp = "";
                    for (int j = 0; j < Main.getOsoby().get(i).getListaKursow().size(); j++) {
                        kursyTemp += Main.getOsoby().get(i).getListaKursow().get(j).getNazwaKursu() + ", ";
                    }
                    if (kursyTemp.length()>=2) {
                        elementy.add(new JLabel("Kursy studenta: " + kursyTemp.substring(0, kursyTemp.length() - 2)), gbc);
                    }
                    else elementy.add(new JLabel("Kursy studenta: "), gbc);
                }
            }

            elementy.add(new JLabel(" "), gbc);
            JButton button0 = new JButton("Wróć");
            button0.addActionListener(new ActionListener() {
                public void actionPerformed(ActionEvent e) {
                    frame.dispose();
                }
            });
            elementy.add(button0, gbc);

            gbc.weighty = 1;
            add(elementy, gbc);
        }
    }
}
