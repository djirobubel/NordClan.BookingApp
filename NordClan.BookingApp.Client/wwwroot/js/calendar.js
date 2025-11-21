let calendar;

window.initCalendar = function (dotNetHelper) {
    const calendarEl = document.getElementById('calendar');
    if (!calendarEl) {
        console.error("Элемент #calendar не найден в DOM!");
        return;
    }

    calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'timeGridWeek',
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        locale: 'ru',
        slotMinTime: '08:00:00',
        slotMaxTime: '20:00:00',
        nowIndicator: true,
        selectable: true,
        selectMirror: true,
        selectOverlap: false,
        slotDuration: '00:15:00',
        snapDuration: '00:15:00',
        height: 'auto',
        expandRows: true,

        slotLabelFormat: {
            hour: '2-digit',
            minute: '2-digit',
            hour12: false
        },
        slotLabelInterval: '00:30:00',

        eventMinHeight: 30,
        eventShortHeight: 30,
        dayMaxEvents: true,
        eventDisplay: 'block',
        eventMinHeight: 45,        
        dayMaxEventRows: 4,      

        select: function (info) {
            dotNetHelper.invokeMethodAsync('OpenModal',
                info.start.toISOString(),
                info.end.toISOString(),
                null);
        },

        eventClick: function (info) {
            const bookingId = parseInt(info.event.id, 10);
            dotNetHelper.invokeMethodAsync('OpenModal',
                info.event.start.toISOString(),
                info.event.end.toISOString(),
                isNaN(bookingId) ? null : bookingId);
        }
    });

    calendar.render();
};

window.updateEvents = function (events) {
    if (!calendar) {
        return;
    }

    calendar.removeAllEvents();

    const eventSource = events.map(e => {
        const color = e.colour || '#3788d8';

        return {
            id: e.id.toString(),
            title: `${e.title} (${e.userLogin})`,
            start: e.startTime,
            end: e.endTime,
            backgroundColor: color,
            borderColor: color,
            textColor: 'white',
            display: 'block',
            extendedProps: {
                description: e.description
            }
        };
    });

    calendar.addEventSource(eventSource);
};