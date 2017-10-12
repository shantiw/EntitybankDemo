// v1.0.0
// requires bootstrapValidator.js
// requires moment.js
if (typeof jQuery === 'undefined') {
    throw new Error('xfront\'s JavaScript requires jQuery');
}

// pagingQuerier options: {url,done(data, page)}
+function ($) {
    "use strict";

    // Class
    var PagingQuerier = function (element, options) {
        this.$element = $(element);
        this.options = $.extend({}, PagingQuerier.DEFAULTS, options);
    };

    PagingQuerier.VERSION = '1.0.0';

    PagingQuerier.DEFAULTS = {
        "url": "/json"
    };

    PagingQuerier.prototype.initialize = function () {
        var $querier = this.$element;
        var $url = this.options.url;
        var $done = this.options.done;

        var searchObj = $.parseObject();
        if (searchObj.pageIndex == null) searchObj.pageIndex = 0;
        $querier.deserializeObject(searchObj);

        var obj = $querier.serializeObject();
        obj.pageIndex = searchObj.pageIndex;

        var r_page = {};
        $.getJSON($url, obj, function (data, textStatus, jqXHR) {

            $querier.find('[data-plugin=renderer]').renderer(data["value"]);

            var pageIndexChange = function (pageIndex) {
                if (pageIndex == 0) {
                    delete searchObj.pageIndex;
                }
                else {
                    searchObj.pageIndex = pageIndex;
                }
                window.location.href = $.toEncodedUrl(searchObj);
            };

            var pageSizeChange = function (name, pageSize) {
                searchObj[name] = pageSize;
                delete searchObj.pageIndex;
                window.location.href = $.toEncodedUrl(searchObj);
            };

            var itemCount = data["@count"];

            var pageIndex = searchObj.pageIndex;

            var pageSize;
            var pageSizer = $querier.find('[data-plugin=pageSizer]');
            if (pageSizer.is('select')) {
                var name = pageSizer.attr('name');
                pageSize = pageSizer.val();
                pageSizer.change(function (event) {
                    event.preventDefault();
                    var size = pageSizer.val();
                    pageSizeChange(name, size);
                });
            }

            var pageCount = Math.ceil(itemCount / pageSize);

            r_page.itemCount = itemCount;
            r_page.pageIndex = pageIndex;
            r_page.pageSize = pageSize;
            r_page.pageCount = pageCount;

            $querier.find('[data-plugin=pagination]').pagination({
                "pageIndex": pageIndex,
                "pageCount": pageCount,
                "change": function (index) {
                    pageIndexChange(index);
                }
            });
            $querier.find('[data-plugin=paginationGo]').paginationGo({
                "pageIndex": pageIndex,
                "pageCount": pageCount,
                "change": function (index) {
                    pageIndexChange(index);
                }
            });
            $querier.find('[data-plugin=paginationInfo]').paginationInfo({
                "pageSize": pageSize,
                "pageIndex": pageIndex,
                "itemCount": itemCount
            });
        }).done(function (data, textStatus, jqXHR) {

            // filterer
            $querier.find('[data-plugin=filterer]').each(function (index, element) {
                var filterer = $(element);
                if (filterer.is(':button')) {
                    var data_name = filterer.attr('data-name');
                    filterer.click(function (event) {
                        event.preventDefault();
                        var elementNames = filterer.attr('data-value-elements').split(',');
                        for (var i = 0; i < elementNames.length; i++) {
                            var elementName = $.trim(elementNames[i]);
                            var element = $('[name=' + elementName + ']');
                            if (element.length > 0) {
                                searchObj[elementName] = element.val();
                            }
                        }
                        if (data_name == null) {
                            delete searchObj.name
                        }
                        else {
                            searchObj.name = data_name;
                        }
                        delete searchObj.pageIndex;
                        window.location.href = $.toEncodedUrl(searchObj);
                    });
                }
            });

            // sorter
            $querier.find('[data-plugin=sorter]').each(function (index, element) {
                var sorter = $(element);
                if (sorter.is('select')) {
                    var name = sorter.attr('name');
                    sorter.change(function (event) {
                        event.preventDefault();
                        var sort = sorter.val();
                        searchObj[name] = sort;
                        delete searchObj.pageIndex;
                        window.location.href = $.toEncodedUrl(searchObj);
                    });
                }
            });

            // tableHeadSorter
            $querier.find('[data-plugin=tableHeadSorter]').tableHeadSorter({
                "change": function (value) {
                    searchObj = $.extend(searchObj, value);
                    delete searchObj.pageIndex;
                    window.location.href = $.toEncodedUrl(searchObj);
                }
            });

            //
            if ($done != null) {
                $done(data["value"], r_page);
            }
        });
    };

    // Plugin
    function Plugin(options) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data('xd.pagingQuerier');
            var opts = typeof options == 'object' && options;
            if (!data) $this.data('xd.pagingQuerier', (data = new PagingQuerier(this, opts)));
            data.initialize();
        })
    }

    var old = $.fn.queryPage;

    $.fn.pagingQuerier = Plugin;
    $.fn.pagingQuerier.Constructor = PagingQuerier;

    // No conflict
    $.fn.pagingQuerier.noConflict = function () {
        $.fn.pagingQuerier = old;
        return this;
    };

}(jQuery);

// $('select[data-name]').initializer()
// initializer options: {url}
+function ($) {
    "use strict";

    // Class
    var Initializer = function (element, options) {
        this.$element = $(element);
        this.options = $.extend({}, Initializer.DEFAULTS, options);
    };

    Initializer.VERSION = '1.0.0';

    Initializer.DEFAULTS = {
        "url": "/json"
    };

    Initializer.prototype.initialize = function () {
        var $el = this.$element;
        var $url = this.options.url;
        var emptyHtml = $el.attr('data-empty-html');
        var name = $el.attr('data-name');
        var data = {};
        data["name"] = name;
        data["key"] = '';
        $.getJSON($url, data, function (data) {
            $el.renderer(data);
            if (emptyHtml != null) {
                var html = '<option selected="selected" value="">' + emptyHtml + '</option>';
                html += $el.html();
                $el.html(html);
            }
        });
    };

    // Plugin
    function Plugin(options) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data('xd.initializer');
            var opts = typeof options == 'object' && options;
            if (!data) {
                $this.data('xd.initializer', (data = new Initializer(this, opts)));

                $.ajaxSettings.async = false;
                data.initialize();
                $.ajaxSettings.async = true;
            }
        })
    }

    var old = $.fn.initializer;

    $.fn.initializer = Plugin;
    $.fn.initializer.Constructor = Initializer;

    // No conflict
    $.fn.initializer.noConflict = function () {
        $.fn.initializer = old;
        return this;
    };

}(jQuery);

//------plugins-------------------------------------------------------------------------------------

// serializeObject() // requires moment.js
+function ($) {
    "use strict";

    var getTimezoneString = function () {
        var offset = new Date().getTimezoneOffset();
        var abs_offset = Math.abs(offset);
        var hours = Math.floor(abs_offset / 60);
        var mins = abs_offset % 60;

        var suffix = (hours < 10) ? '0' + hours : hours.toString();
        suffix += (mins < 10) ? '0' + mins : mins.toString();
        suffix = (offset < 0) ? '+' + suffix : '-' + suffix;
        return suffix;
    }

    var getElementValue = function (element) {
        var value = undefined;

        var plugin = $(element).attr('data-plugin');
        if (plugin == 'datetimepicker') {
            var m = $(element).parent().data("DateTimePicker").date();
            if (m == null) return '';
            var number = m.valueOf();
            var suffix = getTimezoneString();
            value = '/Date(' + number + suffix + ')/';
            return value;
        }

        if ($(element).is('select')) {
            value = $(element).val();
        }

        if ($(element).is('textarea')) {
            value = $(element).val();
            value = value.replace(/\n/gm, '\\r\\n');
        }

        if ($(element).is('input')) {
            var type = $(element).attr('type');
            switch (type) {
                case 'radio':
                    var checked = $(element).prop("checked");
                    if (checked) {
                        value = $(element).val();
                    }
                    else {
                        return null;
                    }
                    break;
                case 'checkbox':
                    var el_value = $(element).attr('value');
                    var checked = $(element).prop("checked");
                    if (el_value == null) {
                        return checked;
                    }
                    else {
                        if (checked) {
                            value = el_value;
                        }
                        else {
                            return null;
                        }
                    }
                    break;
                case 'file':
                case 'image':
                case 'button':
                case 'submit':
                case 'reset':
                    break;
                default:
                    value = $(element).val();
            }
        }

        if (value == '') return value;

        var dataType = $(element).attr('data-data-type');
        if (dataType == 'Date') {
            var format = $(element).attr('data-date-format');
            var m = (format == null) ? moment(value) : moment(value, format);
            var number = m.valueOf();
            var suffix = getTimezoneString();
            value = '/Date(' + number + suffix + ')/';
        }

        if (dataType == 'Number') {
            value = +value;
        }

        if (dataType == 'Boolean') {
            if (value.toString() == 'true') value = true;
            if (value.toString() == 'false') value = false;
        }

        return value;
    };

    var SerializeObject = function (element) {
        var obj = {};
        $(element).find(':input[name]').each(function (index, el) {
            var name = $(el).attr('name');
            var value = getElementValue(el);
            if (value != null) obj[name] = value;
        });
        return obj;
    };

    SerializeObject.VERSION = '1.0.0';

    // Plugin
    var old = $.fn.serializeObject;

    $.fn.serializeObject = function () {
        var result = {};
        this.each(function () {
            var obj = SerializeObject(this);
            result = $.extend({}, result, obj);
        })
        return result;
    };

    // No conflict
    $.fn.serializeObject.noConflict = function () {
        $.fn.serializeObject = old;
        return this;
    };

}(jQuery);

// deserializeObject(obj) // requires moment.js
+function ($) {
    "use strict";

    var setElementValue = function (element, value) {
        var plugin = $(element).attr('data-plugin');
        if (plugin == 'datetimepicker') {
            if (value == null && value == '') {
                $(element).parent().data("DateTimePicker").date(null);
            }
            else {
                var m = moment(value);
                $(element).parent().data("DateTimePicker").date(m);
            }
            return
        }

        var val = value;

        var format = $(element).attr('data-date-format');
        if (format == null) {
            if (isNaN(val) && val.indexOf('/Date(') == 0) {
                var m = moment(val);
                if (m.isValid) {
                    val = m.format('YYYY-MM-DD');
                    if (!moment(val).isSame(m)) {
                        val = m.format('YYYY-MM-DD HH:mm:ss')
                    }
                }
            }
        }
        else {
            var m = moment(val);
            val = m.format(format);
        }

        if ($(element).is('select,textarea')) {
            $(element).val(val);
        }

        if ($(element).is('input')) {
            var type = $(element).attr('type');
            switch (type) {
                case 'radio':
                    var el_val = $(element).val();
                    $(element).prop("checked", el_val == val);
                    break;
                case 'checkbox':
                    var el_value = $(element).attr('value');
                    if (el_value == null) {
                        if (val.toString() == 'true') {
                            $(element).prop("checked", true);
                        }
                        else {
                            $(element).prop("checked", false);
                        }
                    }
                    else {
                        $(element).prop("checked", el_value == val);
                    }
                    break;
                default:
                    $(element).val(val);
                    break;
            }
        }
    }

    var DeserializeObject = function (element, obj) {
        if ($.isArray(obj)) {
            for (var i = 0; i < obj.length; i++) {
                var single = obj[i];
                for (var name in single) {
                    var val = single[name];
                    $(element).find(':checkbox[name=' + name + ']').filter('[value=' + val + ']').prop("checked", true);
                }
            }
            return;
        }

        $(element).find(':input[name]').each(function (index, el) {
            var name = $(el).attr('name');
            var value = obj[name];
            if (value != undefined) setElementValue(el, value);
        });
    };

    DeserializeObject.VERSION = '1.0.0';

    // Plugin
    var old = $.fn.deserializeObject;

    $.fn.deserializeObject = function (obj) {
        return this.each(function () {
            DeserializeObject(this, obj);
        })
    };

    // No conflict
    $.fn.deserializeObject.noConflict = function () {
        $.fn.deserializeObject = old;
        return this;
    };

}(jQuery);

// renderer(data)
+function ($) {
    "use strict";

    // Class
    var Renderer = function (container) {
        this.$container = $(container);
    };

    Renderer.VERSION = '1.0.0';

    var render = function ($container, data) {

        var getHtml = function (template, data) {
            //{{...}}
            var var_statements = '';
            for (var name in data) {
                var value = data[name];
                if (value == null) value = '';

                if (isNaN(value)) {
                    var m = moment(value);
                    if (m.isValid()) {
                        value = m.format('YYYY-MM-DD');
                        if (!moment(value).isSame(m)) {
                            value = m.format('YYYY-MM-DD HH:mm:ss')
                        }
                    }
                }

                var_statements += ' var ' + name + '="' + value + '";';
            }

            var result = template.replace(/\{{2}.*?\}{2}/g, function (word) {
                var exp = word.slice(2, -2);
                exp = var_statements + ' ' + exp;
                return eval(exp);
            });
            return result;
        }

        var html = $container.attr('data-html');
        if (html == null) {
            html = $container.html();
            $container.attr('data-html', html);
            $container.html('');
        }

        if (data == null) return;

        var aggr = '';
        $.each(data, function (index, item) {
            aggr += getHtml(html, item);
        });
        $container.html(aggr);

        $container.find('[data-date-format]').each(function (index, element) {
            var dateFormat = $(element).attr('data-date-format');
            if ($(element).is(':input')) {
                var value = $(element).val();
                if (value != '') {
                    var m = moment(value);
                    value = m.format(dateFormat);
                    $(element).val(value);
                }
            }
            else {
                var text = $(element).text();
                if (text != '') {
                    var m = moment(text);
                    text = m.format(dateFormat);
                    $(element).text(text);
                }
            }
        });

        $container.removeClass('hidden').removeClass('invisible');
    };

    Renderer.prototype.render = function (data) {
        render(this.$container, data);
    };

    // Plugin
    var old = $.fn.renderer;

    $.fn.renderer = function (data) {
        return this.each(function () {
            var $this = $(this);
            var a_data = $this.data('xd.renderer');
            if (!a_data) $this.data('xd.renderer', (a_data = new Renderer(this)));
            a_data.render(data);
        })
    };

    $.fn.renderer.Constructor = Renderer;

    // No conflict
    $.fn.renderer.noConflict = function () {
        $.fn.renderer = old;
        return this;
    };

}(jQuery);

// pagination options: {prev,next,pageIndex,pageCount,change(pageIndex)}
+function ($) {
    "use strict";

    // Class
    var Pagination = function (element, options) {
        this.$element = $(element);

        var data = {};
        var prev = this.$element.attr('data-prev');
        if (prev != null) data.prev = prev;
        var next = this.$element.attr('data-next');
        if (next != null) data.next = next;

        this.options = $.extend({}, Pagination.DEFAULTS, data, options);
    };

    Pagination.VERSION = '1.0.0';

    Pagination.DEFAULTS = {
        "prev": "< Prev",
        "next": "Next >",
        "pageIndex": 0
    };

    Pagination.prototype.setOptions = function (options) {
        this.options = $.extend({}, this.options, options);

        var $el = this.$element;
        var prev = this.options.prev;
        var next = this.options.next;
        var pageIndex = this.options.pageIndex;
        var pageCount = this.options.pageCount;
        var change = this.options.change;

        var ul_html = '';
        if (pageCount == null) {
            ul_html = '';
        }
        else if (pageCount < 11) {
            for (var i = 0; i < pageCount; i++) {
                if (i == pageIndex) {
                    ul_html += '<li class="active"><a href="javascript:">' + (+i + 1) + '</a></li>';
                }
                else {
                    ul_html += '<li><a href="javascript:">' + (+i + 1) + '</a></li>';
                }
            }
        }
        else if (pageIndex < 5) {
            for (var i = 0; i < 6; i++) {
                if (i == pageIndex) {
                    ul_html += '<li class="active"><a href="javascript:">' + (i + 1) + '</a></li>';
                }
                else {

                    ul_html += '<li><a href="javascript:">' + (i + 1) + '</a></li>';
                }
            }
            ul_html += '<li><span>...</span></li>';
            ul_html += '<li><a href="javascript:">' + pageCount + '</a></li>';
            ul_html += '<li><a href="javascript:">' + next + '</a></li>';
        }
        else if (pageCount - pageIndex < 6) {
            ul_html += '<li><a href="javascript:">' + prev + '</a></li>';
            ul_html += '<li><a href="javascript:">1</a></li>';
            ul_html += '<li><span>...</span></li>';
            for (var i = pageCount - 6; i < pageCount; i++) {
                if (i == pageIndex) {
                    ul_html += '<li class="active"><a href="javascript:">' + (+i + 1) + '</a></li>';
                }
                else {

                    ul_html += '<li><a href="javascript:">' + (+i + 1) + '</a></li>';
                }
            }
        }
        else {
            ul_html += '<li><a href="javascript:">' + prev + '</a></li>';
            ul_html += '<li><a href="javascript:">1</a></li>';
            ul_html += '<li><span>...</span></li>';

            ul_html += '<li><a href="javascript:">' + (+pageIndex - 1) + '</a></li>';
            ul_html += '<li><a href="javascript:">' + pageIndex + '</a></li>';
            ul_html += '<li class="active"><a href="javascript:">' + (+pageIndex + 1) + '</a></li>';
            ul_html += '<li><a href="javascript:">' + (+pageIndex + 2) + '</a></li>';
            ul_html += '<li><a href="javascript:">' + (+pageIndex + 3) + '</a></li>';

            ul_html += '<li><span>...</span></li>';
            ul_html += '<li><a href="javascript:">' + pageCount + '</a></li>';
            ul_html += '<li><a href="javascript:">' + next + '</a></li>';
        }

        $el.html(ul_html);

        //
        $el.find('>li:not(.active) a').click(function (event) {
            event.preventDefault();
            var index = $(this).text();
            if (index.indexOf(prev) > -1) {
                index = pageIndex;
            }
            else if (index.indexOf(next) > -1) {
                index = pageIndex + 2;
            }
            if (change != null) {
                change(index - 1);
            }
        });
    };

    // Plugin 
    var old = $.fn.pagination;

    $.fn.pagination = function (options) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data('xd.pagination');
            var opts = typeof options == 'object' && options;
            if (!data) $this.data('xd.pagination', (data = new Pagination(this, opts)));
            data.setOptions(opts);
        })
    };

    $.fn.pagination.Constructor = Pagination;

    // No conflict
    $.fn.pagination.noConflict = function () {
        $.fn.pagination = old;
        return this;
    };

}(jQuery);

// paginationGo options: {pageIndex,pageCount,change(pageIndex)}
+function ($) {
    "use strict";

    // Class
    var PaginationGo = function (element, options) {
        this.$element = $(element);
        var valueElement = this.$element.attr('data-value-element');
        this.$valueElement = $(valueElement);
        if (this.$valueElement.length == 0) {
            this.$valueElement = $('#' + valueElement);
        }
        this.options = $.extend({}, PaginationGo.DEFAULTS, options);

        //
        var instance = this;
        var $el = this.$element;
        $el.click(function (event) {
            event.preventDefault();
            var $ve = instance.$valueElement;
            if ($ve.length == 0) {
                $ve = $('#' + instance.$valueElement);
            }
            var pageCount = instance.options.pageCount;
            var index = $ve.val();
            var isValid = !isNaN(index) && index > 0;
            if (pageCount != null) {
                isValid = isValid && index <= pageCount;
            }
            if (isValid) {
                var change = instance.options.change;
                if (change != null) {
                    change(index - 1);
                }
            }
            else {
                $ve.focus().select();
            }
        });
    };

    PaginationGo.VERSION = '1.0.0';

    PaginationGo.DEFAULTS = {
        "pageIndex": 0
    };

    PaginationGo.prototype.setOptions = function (options) {
        this.options = $.extend({}, this.options, options);
        var pageIndex = this.options.pageIndex;
        var pageCount = this.options.pageCount;

        var $el = this.$element;
        var $ve = this.$valueElement;
        if ($ve.attr('type') == 'number') {
            $ve.attr('min', 1);
            if (pageCount != null) {
                $ve.attr('max', pageCount);
            }
        }

        if (pageCount == null) {
            $ve.val('');
        }
        else {
            var page_index = pageIndex + 2;
            if (page_index > pageCount) page_index = pageCount;
            $ve.val(page_index);
        }
    };

    // Plugin 
    var old = $.fn.paginationGo;

    $.fn.paginationGo = function (options) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data('xd.paginationGo');
            var opts = typeof options == 'object' && options;
            if (!data) $this.data('xd.paginationGo', (data = new PaginationGo(this, opts)));
            data.setOptions(opts);
        })
    };

    $.fn.paginationGo.Constructor = PaginationGo;

    // No conflict
    $.fn.paginationGo.noConflict = function () {
        $.fn.paginationGo = old;
        return this;
    };

}(jQuery);

// paginationInfo options: {pageIndex,itemCount,pageSize}
+function ($) {
    "use strict";

    // Class
    var PaginationInfo = function (element, options) {
        this.$element = $(element);
        this.options = $.extend({}, PaginationInfo.DEFAULTS, options);
    };

    PaginationInfo.VERSION = '1.0.0';

    PaginationInfo.DEFAULTS = {
        "pageIndex": 0
    };

    PaginationInfo.prototype.setOptions = function (options) {
        this.options = $.extend({}, this.options, options);

        var $el = this.$element;

        var html = $el.attr('data-html');
        if (html == null) {
            html = $el.html();
            $el.attr('data-html', html);
            $el.html('');
        }

        var pageIndex = this.options.pageIndex;
        var itemCount = this.options.itemCount;
        var pageSize = this.options.pageSize;
        var pageCount = Math.ceil(itemCount / pageSize);

        if (itemCount == null) return;

        var var_statements = 'var pageIndex=' + pageIndex + ';';
        var_statements += ' var itemCount=' + itemCount + ';';
        var_statements += ' var pageSize=' + pageSize + ';';
        var_statements += ' var pageCount=' + pageCount + ';';

        // {{...}}
        html = html.replace(/\{{2}.*?\}{2}/g, function (word) {
            var exp = word.slice(2, -2);
            exp = var_statements + ' ' + exp;
            return eval(exp);
        });
        $el.html(html);
    };

    // Plugin 
    var old = $.fn.paginationInfo;

    $.fn.paginationInfo = function (options) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data('xd.paginationInfo');
            var opts = typeof options == 'object' && options;
            if (!data) $this.data('xd.paginationInfo', (data = new PaginationInfo(this, opts)));
            data.setOptions(opts);
        })
    };

    $.fn.paginationInfo.Constructor = PaginationInfo;

    // No conflict
    $.fn.paginationInfo.noConflict = function () {
        $.fn.paginationInfo = old;
        return this;
    };

}(jQuery);

// tableHeadSorter options: {value,change(value)} // value: {"header":2,"updown":1}
+function ($) {
    "use strict";

    // Class
    var TableHeadSorter = function (element, options) {
        this.$element = $(element);
        var headerElement = this.$element.attr('data-header-element');
        this.$headerElement = $(headerElement);
        if (this.$headerElement.length == 0) {
            this.$headerElement = $('#' + headerElement);
        }
        var updownElement = this.$element.attr('data-updown-element');
        this.$updownElement = $(updownElement);
        if (this.$updownElement.length == 0) {
            this.$updownElement = $('#' + updownElement);
        }
        this.options = $.extend({}, TableHeadSorter.DEFAULTS, options);

        //
        var instance = this;
        var $th = this.$element.find('th.sort-both, th.sort-asc, th.sort-desc');
        $th.click(function (event) {
            event.preventDefault();
            var $he = instance.$headerElement;
            var $ud = instance.$updownElement;
            var change = instance.options.change;

            if ($(this).hasClass('sort-asc')) {
                $(this).removeClass('sort-asc');
                $(this).addClass('sort-desc');
                var header = $(this).attr('data-header');
                $he.val(header);
                $ud.val(1);
            }
            else if ($(this).hasClass('sort-desc')) {
                $(this).removeClass('sort-desc');
                $(this).addClass('sort-asc');
                var header = $(this).attr('data-header');
                $he.val(header);
                $ud.val(0);
            }
            else if ($(this).hasClass('sort-both')) {
                $th.removeClass('sort-asc').removeClass('sort-desc').addClass('sort-both');
                $(this).removeClass('sort-both');
                $(this).addClass('sort-asc');
                var header = $(this).attr('data-header');
                $he.val(header);
                $ud.val(0);
            }
            else {
                change = null;
            }

            if (change != null) {
                var value = {};
                value[$he.attr('name')] = $he.val();
                value[$ud.attr('name')] = $ud.val();
                change(value);
            }
        });
    };

    TableHeadSorter.VERSION = '1.0.0';

    TableHeadSorter.DEFAULTS = {
    };

    TableHeadSorter.prototype.setOptions = function (options) {
        this.options = $.extend({}, this.options, options);
        var $he = this.$headerElement;
        var $ud = this.$updownElement;
        var value = options.value;
        if (value != null) {
            $he.val(value[$he.attr('name')]);
            var ud = value[$ud.attr('name')];
            if (ud = 'asc') ud = 0;
            if (ud = 'desc') ud = 1;
            $ud.val(ud);
        }

        var $th = this.$element.find('th.sort-both, th.sort-asc, th.sort-desc');
        $th.removeClass('sort-asc').removeClass('sort-desc').addClass('sort-both');
        var header = $he.val();
        var updown = ($ud.val() == 0) ? 'asc' : 'desc';
        $th.filter('[data-header=' + header + ']').removeClass('sort-both').addClass('sort-' + updown);
    };

    // Plugin 
    function Plugin(options) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data('xd.tableHeadSorter');
            var opts = typeof options == 'object' && options;
            if (!data) $this.data('xd.tableHeadSorter', (data = new TableHeadSorter(this, opts)));
            data.setOptions(opts);
        })
    }

    var old = $.fn.tableHeadSorter;

    $.fn.tableHeadSorter = Plugin;
    $.fn.tableHeadSorter.Constructor = TableHeadSorter;

    // No conflict
    $.fn.tableHeadSorter.noConflict = function () {
        $.fn.tableHeadSorter = old;
        return this;
    };

}(jQuery);

//------functions-----------------------------------------------------------------------------------

// $.toEncodedUrl(obj, path)
(function ($) {
    "use strict";

    var ToEncodedUrl = function (obj, path) {
        var search = '';
        $.each(obj, function (index, element) {
            search += index + '=' + element + '&';
        });
        if (search != '') {
            search = search.slice(0, search.length - 1);
            search = '?' + search;
        }

        if (path == null) {
            path = window.location.pathname;
        }
        return encodeURI(path + search);
    };

    ToEncodedUrl.VERSION = '1.0.0';

    $.toEncodedUrl = ToEncodedUrl;

})(jQuery);

// $.parseObject(encodedUrl)
(function ($) {
    "use strict";

    var ParseObject = function (encodedUrl) {
        var url = (encodedUrl == null) ? window.location.href : encodedUrl;
        url = decodeURI(url);

        var obj = {};
        var index = url.indexOf('?');
        if (index == -1) return obj;
        var search = url.slice(index + 1);
        var array = search.split('&');
        for (var i = 0; i < array.length; i++) {
            var pair = array[i].split('=');
            var key = pair[0];
            var value = pair[1];
            obj[key] = value;
        }
        return obj;
    };

    ParseObject.VERSION = '1.0.0';

    $.parseObject = ParseObject;

})(jQuery);

//------modification--------------------------------------------------------------------------------

// $.put(url, data, dataType)
(function ($) {
    "use strict";

    var Put = function (url, data, dataType) {
        return $.ajax({
            "async": true,
            "url": url,
            "type": "PUT",
            "contentType": "application/json",
            "data": JSON.stringify(data),
            "dataType": (dataType == null) ? "json" : dataType
        });
    };

    Put.VERSION = '1.0.0';

    $.put = Put;

})(jQuery);

// $.delete(url, data, dataType)
(function ($) {
    "use strict";

    var Delete = function (url, data, dataType) {
        return $.ajax({
            "async": true,
            "url": url,
            "type": "DELETE",
            "contentType": "application/json",
            "data": JSON.stringify(data),
            "dataType": (dataType == null) ? "json" : dataType
        });
    };

    Delete.VERSION = '1.0.0';

    $.delete = Delete;

})(jQuery);

// errorRenderer(error)
+function ($) {
    "use strict";

    // Class
    var ErrorRenderer = function (element) {
        this.$element = $(element);
    };

    ErrorRenderer.VERSION = '1.0.0';

    ErrorRenderer.prototype.render = function (error) {
        var html = '';
        if ($.isArray(error)) {
            $.each(error, function (index, err) {
                html += '<li>' + err.ErrorMessage + '</li>';
            });
        }
        else {
            html = '<li>' + error.ExceptionMessage + '</li>';
        }

        var $el = this.$element;
        if (!$el.is('ul,ol')) {
            html = '<ul>' + html + '</ul>';
        }

        html = '<br />' + html;
        $el.html(html);
    };

    // Plugin
    var old = $.fn.errorRenderer;

    $.fn.errorRenderer = function (error) {
        return this.each(function () {
            var $this = $(this);
            var data = $this.data('xd.errorRenderer');
            if (!data) $this.data('xd.errorRenderer', (data = new ErrorRenderer(this)));
            data.render(error);
        })
    };

    $.fn.errorRenderer.Constructor = ErrorRenderer;

    // No conflict
    $.fn.errorRenderer.noConflict = function () {
        $.fn.errorRenderer = old;
        return this;
    };

}(jQuery);
