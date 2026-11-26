const profileEditButtons = document.querySelectorAll('.profile-edit-button');

profileEditButtons.forEach(button => {
    button.addEventListener('click', (e) => {
       e.preventDefault();
       const input = button.previousElementSibling;
       const wasEmail = input.type === 'email';
       input.type = 'text';
       input.removeAttribute('disabled');
       let textLength = button.previousElementSibling.value.length;
       input.setSelectionRange(textLength, textLength);
       input.focus();
        if (wasEmail) {
            input.type = 'email';
        }
    });
});