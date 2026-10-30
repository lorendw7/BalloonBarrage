// Ephemeral discussion choices: no network request, storage or plan mutation.
const form = document.querySelector("[data-world-choices]");
if (form) {
  const panel = form.querySelector(".world-choice-summary");
  const output = form.querySelector("output");
  const update = () => {
    output.textContent = [...form.querySelectorAll("fieldset")].map(fieldset => {
      const selected = fieldset.querySelector("input:checked");
      return `${fieldset.dataset.label}: ${selected ? selected.closest("label").querySelector("span").textContent : form.dataset.unset}`;
    }).join("\n");
  };
  form.addEventListener("change", update);
  // Reset's default action runs after its event listeners; update on the next task.
  form.addEventListener("reset", () => setTimeout(update, 0));
  form.addEventListener("submit", event => event.preventDefault());
  update();
  panel.hidden = false;
}
