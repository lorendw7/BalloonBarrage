// Optional enhancements; navigation and download links work without JavaScript.
const dialog = document.querySelector(".art-dialog");
let opener;
for (const link of document.querySelectorAll("[data-art]")) {
  link.addEventListener("click", (event) => {
    if (!dialog?.showModal || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey) return;
    event.preventDefault();
    opener = link;
    const image = link.querySelector("img");
    const enlarged = dialog.querySelector("img");
    enlarged.src = link.href;
    enlarged.alt = image.alt;
    dialog.querySelector("p").textContent = link.closest("figure").querySelector("figcaption span").textContent;
    dialog.showModal();
  });
}
dialog?.querySelector("button").addEventListener("click", () => dialog.close());
dialog?.addEventListener("close", () => opener?.focus());
dialog?.addEventListener("click", (event) => {
  if (event.target !== dialog) return;
  const box = dialog.getBoundingClientRect();
  if (event.clientX < box.left || event.clientX > box.right || event.clientY < box.top || event.clientY > box.bottom) dialog.close();
});
