
document.addEventListener("DOMContentLoaded", () => {
   let form = document.querySelector("#editForm");
   form.addEventListener("submit", event => {
      event.preventDefault();
      //получаем имя контроллера
      let controller = form.dataset.controller; 
      //id объекта
      let id = form.elements["formId"].value;
      let formData = new FormData(form);
      // получаем элемент формы returnElem или null
      let returnUrlElement = form.elements["returnUrl"] == null ? null : form.elements["returnUrl"].value;
      // получаем значение элемента returnELem или null
      let returnUrl = returnUrlElement ? encodeURIComponent(returnUrlElement) : null;
      // создаем ссылку для возвращения к меню Details
      let redirectUrl = returnUrlElement ? "/" + controller + `/Details/${id}` + `/${returnUrl}` : "/" + controller + `/Details/${id}`   
      // ссылка запроса к методу Edit
      let url = "/" + controller + '/Edit/' + `${id}` 
     
      fetch(url, {
      method: "POST",
      body: formData
      }).then(response => {
            //при успешном запросе возвращаем информацию измененного объекта
            if (response.ok) {                  
               location = redirectUrl               
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
