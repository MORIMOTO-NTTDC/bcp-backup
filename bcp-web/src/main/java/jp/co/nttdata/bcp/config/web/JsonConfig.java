package jp.co.nttdata.bcp.config.web;

import java.io.IOException;
import java.util.List;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.EnableAspectJAutoProxy;
import org.springframework.http.converter.HttpMessageConverter;
import org.springframework.http.converter.json.Jackson2ObjectMapperBuilder;
import org.springframework.http.converter.json.MappingJackson2HttpMessageConverter;
import org.springframework.util.StringUtils;
import org.springframework.web.servlet.config.annotation.EnableWebMvc;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

import com.fasterxml.jackson.core.JacksonException;
import com.fasterxml.jackson.core.JsonGenerator;
import com.fasterxml.jackson.core.JsonParser;
import com.fasterxml.jackson.databind.DeserializationContext;
import com.fasterxml.jackson.databind.JsonSerializer;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.SerializerProvider;
import com.fasterxml.jackson.databind.deser.std.StdDeserializer;
import com.fasterxml.jackson.databind.deser.std.StringDeserializer;
import com.fasterxml.jackson.databind.module.SimpleModule;
import com.fasterxml.jackson.databind.ser.std.StdSerializer;

/**
 * SpringMVCのonfigにJSON絡みの設定を追加.
 */
@ComponentScan(basePackages = { "jp.co.nttdata.bcp.app" })
@EnableAspectJAutoProxy
@EnableWebMvc
@Configuration
public class JsonConfig implements WebMvcConfigurer {

	/**
	 * {@inheritDoc}
	 */
	@Override
	public void extendMessageConverters(
			List<HttpMessageConverter<?>> converters) {

		MappingJackson2HttpMessageConverter converter = null;
		// 定義済みのMappingJackson2HttpMessageConverterを取得
		for (HttpMessageConverter<?> httpMessageConverter : converters) {
			if (httpMessageConverter instanceof MappingJackson2HttpMessageConverter) {
				converter = (MappingJackson2HttpMessageConverter) httpMessageConverter;
				break;
			}
		}

		converter.setObjectMapper(objectMapper());
	}

	@Bean
	public ObjectMapper objectMapper() {

		SimpleModule module = new SimpleModule();

		// 空文字をNULLに変換するデシリアライザーを追加
		module.addDeserializer(String.class, new StringDeserializer() {
			@Override
			public String deserialize(JsonParser p, DeserializationContext ctxt) throws IOException {
				String result = super.deserialize(p, ctxt);
				if (StringUtils.hasText(result)) {
					return result.trim();
				} else {
					return null;
				}
			}
		});

		// Booleanをtrue,falseに限定するデシリアライザーを追加
		module.addDeserializer(Boolean.class, new StdDeserializer<>(Boolean.class) {
			@Override
			public Boolean deserialize(JsonParser p, DeserializationContext ctxt) throws IOException, JacksonException {
				String text = p.getText();
				if (!StringUtils.hasText(text)) {
					return null;
				}
				text = text.trim();
				if ("true".equalsIgnoreCase(text) || "false".equalsIgnoreCase(text)) {
					return Boolean.valueOf(text);
				} else {
					return (Boolean) ctxt.handleWeirdStringValue(_valueClass, text,
							"only \"true\" or \"false\" recognized");
				}
			}
		});

		// Integerを半角数値に限定するデシリアライザーを追加
		module.addDeserializer(Integer.class, new StdDeserializer<>(Integer.class) {
			@Override
			public Integer deserialize(JsonParser p, DeserializationContext ctxt) throws IOException, JacksonException {
				Object text = isHalfNumber(p, ctxt, _valueClass);
				return text == null ? null : _parseInteger(ctxt, (String) text);
			}
		});
		// Longを半角数値に限定するデシリアライザーを追加
		module.addDeserializer(Long.class, new StdDeserializer<>(Long.class) {
			@Override
			public Long deserialize(JsonParser p, DeserializationContext ctxt) throws IOException, JacksonException {
				Object text = isHalfNumber(p, ctxt, _valueClass);
				return text == null ? null : _parseLong(ctxt, (String) text);
			}
		});

		// 数値を文字列として出力するシリアライザーを追加
		module.addSerializer(new StdSerializer<>(Number.class) {
		    @Override
		    public void serialize(Number value, JsonGenerator gen, SerializerProvider provider)
		            throws IOException {
		        gen.writeString(value.toString());
		    }
		});
		
		// Booleanを文字列として出力するシリアライザーを追加
		module.addSerializer(new StdSerializer<>(Boolean.class) {
		    @Override
		    public void serialize(Boolean value, JsonGenerator gen, SerializerProvider provider)
		            throws IOException {
		        gen.writeString(value.toString());
		    }
		});

		
		ObjectMapper objectMapper = Jackson2ObjectMapperBuilder.json().build();
		objectMapper.registerModule(module);

		// NULLを空文字として出力する
		objectMapper.getSerializerProvider().setNullValueSerializer(new JsonSerializer<Object>() {
		    @Override
		    public void serialize(Object value, JsonGenerator gen, SerializerProvider serializers)
		            throws IOException {
		        gen.writeString("");
		    }
		});
		
		return objectMapper;

	}

	private static Object isHalfNumber(JsonParser p, DeserializationContext ctxt, Class<?> _valueClass)
			throws IOException {
		String text = p.getText();
		if (!StringUtils.hasText(text)) {
			return null;
		}
		text = text.trim();
		if (!text.matches("^(\\+|\\-)?[0-9]+$")) {
			return ctxt.handleWeirdStringValue(_valueClass, text,
					"半角数値以外を含む");
		}
		return text;
	}

}