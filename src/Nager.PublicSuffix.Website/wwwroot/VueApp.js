var app = new Vue({
    el: '#app',
    data: {
        report: null,
        domain: 'mail.example.com'
    },
    created() {
        this.analyze();
    },
    methods: {
        analyze() {
            axios.get(`api/publicsuffix?domain=${this.domain}`)
                .then(response => {
                    // JSON responses are automatically parsed.
                    this.report = response.data;
                })
                .catch(exception => {
                    console.log(exception);
                })
        }
    }
});