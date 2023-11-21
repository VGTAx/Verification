
document.addEventListener("DOMContentLoaded", () => {
   let form = document.querySelector("#filterForm");
   form.addEventListener("submit", event => {
      event.preventDefault();
      
      //получаем имя контроллера
      let controller = form.dataset.controller; 
      // получаем метод
      let action = event.submitter.dataset.action;
      //получаем содержимое формы
      let formData = new FormData(form);
      //удаляем токен
      formData.delete('__RequestVerificationToken');
      //получаем параметры
      let params = new URLSearchParams(formData).toString()
      //создаем ссылку Index
      let fullUrl = "/" + controller + `/${action}/?&` + `${params}`;     
      fetch(fullUrl, {
      method: "Get",     
      }).then(response => {
            //при успешном запросе возвращаем список
            if (response.ok) {               
               location = "/" + controller + `/${action}/?&`+ `${params}`
            }
            //если были переданы некорректные данные - обрабатываем ошибку и выводим текст в форму
            if (response.status == 400) {
               return response.json().then(data => {
               let spanAll = form.querySelectorAll("span");
               spanAll.forEach(c => c.textContent = "");
                  if (data) {
                     for (let key in data) {
                        let textError = data[key].join(", ");
                        document.querySelector("#" + `${key}` + "Span").textContent = textError;
                  }
               }
            })
         }
      })
   })
})
