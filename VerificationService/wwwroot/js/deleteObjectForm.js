
document.addEventListener("DOMContentLoaded", () => {
   let form = document.querySelector("#deleteForm");
   form.addEventListener("submit", event => {
      event.preventDefault();
      //получаем имя контроллера
      let controller = form.dataset.controller; 
      //id объекта
      let id = form.elements["formId"].value;
      let formData = new FormData(form);
      // получаем значение элемента returnUrl (ссылку для возврата в метод Index c параметрами фильтра или без)
      let returnUrl = form.elements["returnUrl"].value;
      // ссылка для удаления проверки
      let url = "/" + controller + '/Delete/' + `${id}` 
     
      fetch(url, {
      method: "POST",
      body: formData
      }).then(response => {
            //при успешном запросе возвращаемся в Index
            if (response.ok) {               
               location = returnUrl               
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
