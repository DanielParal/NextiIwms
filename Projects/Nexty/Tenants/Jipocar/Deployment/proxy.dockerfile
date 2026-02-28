FROM nginx:latest

# Switch to root for timezone setup
USER root

COPY proxy-ssl.config /etc/nginx/conf.d/ssl-common.conf
COPY ssl/fullchain.pem /etc/nginx/ssl/fullchain.pem
COPY ssl/privkey.pem /etc/nginx/ssl/privkey.pem
COPY proxy.config /etc/nginx/templates/proxy.config.template

# Add environment variables to config at runtime
CMD ["/bin/sh", "-c", "envsubst '${API_SERVER_NAME} ${FE_ADMIN_SERVER_NAME} ${FE_VHLA_SERVER_NAME} ${FE_SIGN_SERVER_NAME} ${INFO_FE_SERVER_NAME} ${INFO_BE_SERVER_NAME}' < /etc/nginx/templates/proxy.config.template > /etc/nginx/conf.d/default.conf && nginx -g 'daemon off;'"]