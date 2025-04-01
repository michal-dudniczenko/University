const canvases = document.querySelectorAll(".drawingX");

canvases.forEach((canvas) => {
  const ctx = canvas.getContext("2d");
  
  function resizeCanvas() {
    canvas.width = canvas.clientWidth;
    canvas.height = canvas.clientHeight;
  }

  function drawLinesToMouse(x, y) {
    ctx.strokeStyle = "red";
    ctx.lineWidth = 2;  

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    ctx.beginPath();
    ctx.moveTo(0, 0);
    ctx.lineTo(x, y);

    ctx.moveTo(canvas.width, 0);
    ctx.lineTo(x, y);

    ctx.moveTo(0, canvas.height); 
    ctx.lineTo(x, y);

    ctx.moveTo(canvas.width, canvas.height);
    ctx.lineTo(x, y);

    ctx.stroke();
  }

  canvas.addEventListener("mousemove", (event) => {
    const rect = canvas.getBoundingClientRect();
    const mouseX = event.clientX - rect.left;
    const mouseY = event.clientY - rect.top;
    drawLinesToMouse(mouseX, mouseY);
  });

  canvas.addEventListener("mouseleave", () => {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
  });

  resizeCanvas();
  window.addEventListener("resize", resizeCanvas);
});
