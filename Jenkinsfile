properties([pipelineTriggers([githubPush()])])

pipeline {
    agent { dockerfile true }
    environment {
        PROJECT_KEY = "vtb-templatesservice"
        SONARQUBE_TOKEN = credentials('SONARQUBE_TOKEN');
        SONARQUBE_URL = credentials('SONARQUBE_URL');

        BAGET_USERNAME = credentials('BAGET_USERNAME');
        BAGET_PASSWORD = credentials('BAGET_PASSWORD');
        BAGET_HOST = credentials('BAGET_HOST');
        BAGET_API_KEY = credentials('BAGET_API_KEY');

        REGISTRY_HOST = credentials('REGISTRY_HOST');
        REGISTRY_USERNAME = credentials('REGISTRY_USERNAME');
        REGISTRY_PASSWORD = credentials('REGISTRY_PASSWORD');

        BRANCH_NAME = "${env.BRANCH_NAME}";
        VERSION = "1.0.${BUILD_NUMBER}"

        COMMIT_HASH = "${GIT_COMMIT}"
        IMAGE_NAME = "${REGISTRY_HOST}/${PROJECT_KEY}:${BRANCH_NAME}-latest"
    }

    stages {
        stage('SCA Setup') {
            steps {
                sh '''\
                    dotnet sonarscanner begin \
                        /k:"${PROJECT_KEY}" \
                        /d:sonar.login="${SONARQUBE_TOKEN}" \
                        /d:sonar.host.url="${SONARQUBE_URL}" \
                        /d:sonar.coverage.exclusions="*.Tests/" \
                        /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml"
                    '''
            }
        }
        stage('Restore') {
            steps {
                sh 'dotnet nuget add source https://${BAGET_HOST}/v3/index.json --username ${BAGET_USERNAME} --password ${BAGET_PASSWORD} --store-password-in-clear-text --name BaGet'
                sh 'dotnet restore'
            }
        }
        stage('Build') {
            steps {
                sh 'dotnet build -p:Version=${VERSION} -c Release --no-restore'
            }
        }
        stage('Run All Tests') {        
            steps {
                // workaround to run tests sequentially as Mongo2Go doesn't really like parallelism
                sh '''\
                    dotnet test vtb.TemplatesService.BusinessLogic.Tests/vtb.TemplatesService.BusinessLogic.Tests.csproj \
                        -c Release \
                        --no-build \
                        --logger "trx" \
                        /p:CollectCoverage=true \
                        /p:CoverletOutputFormat="opencover" \
                        /p:CoverletOutput=vtb.TemplatesService.BusinessLogic.Tests/
                '''
                sh '''\
                    dotnet test vtb.TemplatesService.DataAccess.Tests/vtb.TemplatesService.DataAccess.Tests.csproj \
                        -c Release \
                        --no-build \
                        --logger "trx" \
                        /p:CollectCoverage=true \
                        /p:CoverletOutputFormat="opencover" \
                        /p:CoverletOutput=vtb.TemplatesService.DataAccess.Tests/
                '''
                sh '''\
                    dotnet test vtb.TemplatesService.Api.Tests/vtb.TemplatesService.Api.Tests.csproj \
                        -c Release \
                        --no-build \
                        --logger "trx" \
                        /p:CollectCoverage=true \
                        /p:CoverletOutputFormat="opencover" \
                        /p:CoverletOutput=vtb.TemplatesService.Api.Tests/
                '''
                mstest()
            }
        }
        stage('SCA Upload') {
            steps {
                sh 'dotnet sonarscanner end /d:sonar.login="${SONARQUBE_TOKEN}"'
            }
        }
        stage('Dockerize') {
            steps {
                sh 'dotnet publish vtb.TemplatesService.Api/vtb.TemplatesService.Api.csproj --no-build --no-restore -o ./out -c Release'

                dir('out'){
                    sh 'docker login ${REGISTRY_HOST} --username ${REGISTRY_USERNAME} --password ${REGISTRY_PASSWORD}'
                    sh 'docker build . -f ../Dockerfile.API -t ${IMAGE_NAME}'
                    sh 'docker push ${IMAGE_NAME}'
                }
            }
        }
        stage('Run Release') {
            when {
                expression { BRANCH_NAME == 'master' }
            }
            steps {             
                build job: 
                    'Release-vtb.TemplatesService', 
                    parameters: [
                        string(name: 'DockerImageTag', value: '${IMAGE_NAME}'), 
                        string(name: 'BranchName', value: '${BRANCH_NAME}')
                    ]
            }
        }
    }

    post {
        cleanup {
            echo "Cleaning up"
            sh 'rm -rfv ./*'
            sh 'dotnet nuget remove source BaGet'
        }
    }
}