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
    dialog.querySelector("p").textContent =
      link.closest("figure")?.querySelector("figcaption span")?.textContent ?? image.alt;
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

// The full art library stays visible without JavaScript. Filters enhance it in place.
const gallery = document.querySelector("#art-grid");
const toolbar = document.querySelector("[data-gallery-toolbar]");
if (gallery && toolbar) {
  const cards = [...gallery.querySelectorAll("[data-art-category]")];
  const filters = [...toolbar.querySelectorAll("[data-art-filter]")];
  const status = toolbar.querySelector("[data-art-count]");
  const filterArt = (selected) => {
    let visible = 0;
    for (const card of cards) {
      card.hidden = selected !== "all" && card.dataset.artCategory !== selected;
      if (!card.hidden) visible += 1;
    }
    for (const button of filters) button.setAttribute("aria-pressed", String(button.dataset.artFilter === selected));
    status.textContent = status.dataset.countTemplate.replace("{count}", visible).replace("{total}", cards.length);
  };
  for (const button of filters) button.addEventListener("click", () => filterArt(button.dataset.artFilter));
  filterArt("all");
  toolbar.hidden = false;
}
