console.log("PowerToys Run ChatGPT Helper script loaded");

const searchParams = new URLSearchParams(window.location.search);
const prompt = searchParams.get("PTquery");
if (prompt) {
  const textArea = document.querySelector("form textarea");
  const submitButton = document.querySelector("form button");

  if (!textArea || !submitButton) {
    console.error("Cannot find required elements");
  }

  textArea.value = prompt;
  setTimeout(() => {
    textArea.value = prompt;
    submitButton.click();
  }, 0);
}
