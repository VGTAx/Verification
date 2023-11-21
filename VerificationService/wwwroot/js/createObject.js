document.addEventListener("DOMContentLoaded", () => {

   //получаем ссылку на форму
   let links = document.querySelector(".linkForm");

   links.addEventListener('click', (event) => {
      //определяем форму, которая была вызвана
      let targetForm = event.target.dataset.target;
      let form = document.querySelector("#" + `${targetForm}`);
      //вешаем обработчик
      form.addEventListener("submit", event => {
         event.preventDefault();
         //получаем адрес контроллер + таргет(дестйвие)
         let controller = form.dataset.controller;
         let target = form.dataset.target;

         let formData = new FormData(form);
         //отправляем запрос
         fetch("/" + controller + "/" + target, {
            method: "POST",
            body: formData
         }).then(response => {
            //при успешном запросе возвращаемся на страницу со списком объектов
            if (response.ok) {
               location = "/" + controller + "/Index";
            }
            //обработка ошибки. статус 400 - некорректные данные.            
            if (response.status == 400) {
               return response.json().then(data => {
                  let spanAll = form.querySelectorAll("span");
                  spanAll.forEach(c => c.textContent = "");
                  if (data) {
                     //выводим ошибки в форму
                     for (let key in data) {
                        let textError = data[key].join(", ");
                        let span = document.querySelector("#" + `${key}` + "Span");
                        span.textContent = textError;
                     }
                  }
               })
            }

         })
      });
   });
})