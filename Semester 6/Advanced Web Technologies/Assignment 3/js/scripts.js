function updateScrollPadding() {
  const navbar = document.getElementById("navbar");
  if (navbar) {
    const navbarHeight = navbar.offsetHeight;
    document.documentElement.style.scrollPaddingTop = navbarHeight - 1 + "px";
  }
}

let chartInstance;

function createChart() {
  const ctx = document.getElementById("myChart");

  if (chartInstance) {
    chartInstance.destroy();
  }

  const x = [
    "Francja", 
    "Hiszpania", 
    "Włochy", 
    "USA", 
    "Tajlandia", 
    "Japonia", 
    "Grecja", 
    "Turcja", 
    "Meksyk", 
    "Szwecja"
  ];

  const y = [
    89,
    83,
    79,
    62,
    51,
    39,
    34,
    32,
    21,
    14
  ];
  
  const data = {
    labels: x,
    datasets: [{
      axis: "y",
      label: "Liczba turystów (mln)",
      data: y,
      fill: false,
      backgroundColor: [
        "rgba(255, 99, 132, 0.2)",
        "rgba(255, 159, 64, 0.2)",
        "rgba(255, 205, 86, 0.2)",
        "rgba(75, 192, 192, 0.2)",
        "rgba(54, 162, 235, 0.2)",
        "rgba(153, 102, 255, 0.2)",
        "rgba(201, 203, 207, 0.2)",
        "rgba(0, 255, 127, 0.2)",
        "rgba(255, 0, 255, 0.2)",
        "rgba(255, 69, 0, 0.2)"
      ],
      borderColor: [
          "rgb(255, 99, 132)",
          "rgb(255, 159, 64)",
          "rgb(255, 205, 86)",
          "rgb(75, 192, 192)",
          "rgb(54, 162, 235)",
          "rgb(153, 102, 255)",
          "rgb(201, 203, 207)",
          "rgb(0, 255, 127)",
          "rgb(255, 0, 255)",
          "rgb(255, 69, 0)"
      ],
      borderWidth: 1
    }]
  };
  
  const config = {
    type: "bar",
    data,
    options: {
      indexAxis: "y",
      scales: {
        y: {
          beginAtZero: true
        }
      }
    }
  };
  
  chartInstance = new Chart(ctx, config);
}

document.addEventListener("DOMContentLoaded", createChart);
document.addEventListener("DOMContentLoaded", updateScrollPadding);

window.addEventListener("resize", updateScrollPadding);
window.addEventListener('resize', createChart);
